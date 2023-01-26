using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerCore;

namespace Server
{
    struct JobTimerElement : IComparable<JobTimerElement>
    {
        public int execTick;
        public Action action;

        public int CompareTo(JobTimerElement other)
        {
            return other.execTick - this.execTick;
        }
    }

    class JobTimer
    {
        PriorityQueue<JobTimerElement> _pq = new PriorityQueue<JobTimerElement>();
        object _lock = new object();

        public static JobTimer Instance { get; } = new JobTimer();

        public void Push(Action action, int tickAfter = 0)
        {
            JobTimerElement job;
            job.execTick = System.Environment.TickCount + tickAfter;
            job.action = action;

            lock (_lock)
            {
                _pq.Push(job);
            }
        }

        public void Flush()
        {
            while (true)
            {
                int now = System.Environment.TickCount;

                JobTimerElement job;

                lock (_lock)
                {
                    if (_pq.Count == 0)
                        break;

                    job = _pq.Peek();
                    if (job.execTick > now)
                        break;

                    _pq.Pop();
                }

                job.action.Invoke();
            }
        }
    }
}
