using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    class Lock
    {
        const int EMPTY_FLAG = 0x00000000;
        const int WRITE_MASK = 0x7FFF0000;
        const int READ_MASK = 0x0000FFFF;
        const int MAX_SPIN_COUNT = 5000;

        // [Unused(1)] [WriteThreadID(15)] [ReadCount(16)]
        int flag;
        int writeCount;

        public void WriteLock()
        {
            int lockThreadID = (flag & WRITE_MASK) >> 16;
            if (writeCount == Thread.CurrentThread.ManagedThreadId)
            {
                writeCount++;
                return;
            }

            int desired = (Thread.CurrentThread.ManagedThreadId << 16) & WRITE_MASK;
            while (true)
            {
                for (int i = 0; i < MAX_SPIN_COUNT; i++)
                {
                    if (Interlocked.CompareExchange(ref flag, desired, EMPTY_FLAG) == EMPTY_FLAG)
                    {
                        writeCount = 1;
                        return;
                    }
                }

                Thread.Yield();
            }
        }

        public void WriteUnlock()
        {
            int lockCount = --writeCount;
            if (lockCount == 0)
                Interlocked.Exchange(ref flag, EMPTY_FLAG);
        }

        public void ReadLock()
        {
            int lockThreadID = (flag & WRITE_MASK) >> 16;
            if (writeCount == Thread.CurrentThread.ManagedThreadId)
            {
                Interlocked.Increment(ref flag);
                return;
            }

            while (true)
            {
                for (int i = 0; i < MAX_SPIN_COUNT; i++)
                {
                    int expected = (flag & READ_MASK);
                    if (Interlocked.CompareExchange(ref flag, expected + 1, expected) == expected)
                        return;
                }

                Thread.Yield();
            }
        }

        public void ReadUnlock()
        {
            Interlocked.Decrement(ref flag);
        }
    }
}
