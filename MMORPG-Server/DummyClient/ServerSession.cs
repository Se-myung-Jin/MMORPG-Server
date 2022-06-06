using System.Text;
using ServerCore;
using System.Net;

namespace DummyClient
{
    public class Packet
    {
        public ushort size;
        public ushort packedId;
    }

    class ServerSession : Session
    {
        public override void OnConnected(EndPoint _endPoint)
        {
            Console.WriteLine($"OnConnected : {_endPoint}");

            Packet packet = new Packet() { size = 4, packedId = 7 };

            for (int i = 0; i < 5; i++)
            {
                ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);
                byte[] buffer = BitConverter.GetBytes(packet.size);
                byte[] buffer2 = BitConverter.GetBytes(packet.packedId);
                Array.Copy(buffer, 0, openSegment.Array, openSegment.Offset, buffer.Length);
                Array.Copy(buffer2, 0, openSegment.Array, openSegment.Offset + buffer.Length, buffer2.Length);
                ArraySegment<byte> sendBuffer = SendBufferHelper.Close(packet.size);

                Send(sendBuffer);
            }
        }

        public override void OnDisconnected(EndPoint _endPoint)
        {
            Console.WriteLine($"OnDisconnected : {_endPoint}");
        }

        public override int OnRecv(ArraySegment<byte> _buffer)
        {
            string recvData = Encoding.UTF8.GetString(_buffer.Array, _buffer.Offset, _buffer.Count);

            Console.WriteLine($"[From Server] {recvData}");

            return _buffer.Count;
        }

        public override void OnSend(int _numOfBytes)
        {
            Console.WriteLine($"Transferred bytes: {_numOfBytes}");
        }
    }
}
