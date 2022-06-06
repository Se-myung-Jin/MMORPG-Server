using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public class RecvBuffer
    {
        ArraySegment<byte> buffer;
        int readPos;
        int writePos;

        public RecvBuffer(int _bufferSize)
        {
            buffer = new ArraySegment<byte>(new byte[_bufferSize], 0, _bufferSize);
        }

        public int DataSize { get { return writePos - readPos; } } // 쓰여진 공간
        public int FreeSize { get { return buffer.Count - writePos; } } // 남은 공간

        public ArraySegment<byte> ReadSegment
        {
            get { return new ArraySegment<byte>(buffer.Array, buffer.Offset + readPos, DataSize); }
        }
        public ArraySegment<byte> WriteSegment
        {
            get { return new ArraySegment<byte>(buffer.Array, buffer.Offset + writePos, FreeSize); }
        }

        public void Clean()
        {
            int dataSize = DataSize;
            if (dataSize == 0)
            {
                // 남은 데이터가 없으면 복사하지 않고 커서 위치만 리셋
                readPos = writePos = 0;
            }
            else
            {
                // 남은 데이터가 있으면 시작 위치로 복사
                Array.Copy(buffer.Array, buffer.Offset + readPos, buffer.Array, buffer.Offset, dataSize);
                readPos = 0;
                writePos = dataSize;
            }
        }

        public bool OnRead(int _numOfBytes)
        {
            if (_numOfBytes > DataSize)
            {
                return false;
            }

            readPos += _numOfBytes;

            return true;
        }

        public bool OnWrite(int _numOfBytes)
        {
            if (_numOfBytes > FreeSize)
            {
                return false;
            }

            writePos += _numOfBytes;

            return true;
        }
    }
}
