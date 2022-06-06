namespace ServerCore
{
    public class SendBufferHelper
    {
        public static ThreadLocal<SendBuffer> CurrentBuffer = new ThreadLocal<SendBuffer>(() => { return null; });

        public static int ChunkSize { get; set; } = 4096 * 100;

        public static ArraySegment<byte> Open(int _reserveSize)
        {
            if (CurrentBuffer.Value == null)
                CurrentBuffer.Value = new SendBuffer(ChunkSize);

            if (CurrentBuffer.Value.FreeSize < _reserveSize)
                CurrentBuffer.Value = new SendBuffer(ChunkSize);

            return CurrentBuffer.Value.Open(_reserveSize);
        }

        public static ArraySegment<byte> Close(int _usedSize)
        {
            return CurrentBuffer.Value.Close(_usedSize);
        }
    }
    public class SendBuffer
    {
        byte[] buffer;
        int usedSize = 0;

        public int FreeSize { get { return buffer.Length - usedSize; } } // 남은 공간

        public SendBuffer(int _chunkSize)
        {
            buffer = new byte[_chunkSize];
        }
        
        public ArraySegment<byte> Open(int _reserveSize)
        {
            if (_reserveSize > FreeSize)
                return null;

            return new ArraySegment<byte>(buffer, usedSize, _reserveSize);
        }
        public ArraySegment<byte> Close(int _usedSize)
        {
            ArraySegment<byte> segment = new ArraySegment<byte>(buffer, usedSize, _usedSize);
            usedSize += _usedSize;

            return segment;
        }
    }
}
