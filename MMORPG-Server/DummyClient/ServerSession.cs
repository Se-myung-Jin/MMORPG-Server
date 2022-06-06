using System.Text;
using ServerCore;
using System.Net;

namespace DummyClient
{
    public abstract class Packet
    {
        public ushort size;
        public ushort packedId;

        public abstract ArraySegment<byte> Serialize();
        public abstract void Deserialize(ArraySegment<byte> _s);
    }

    class PlayerInfoReq : Packet
    {
        public long playerId;

        public PlayerInfoReq()
        {
            packedId = (ushort)PacketID.PlayerInfoReq;
        }

        public override void Deserialize(ArraySegment<byte> _s)
        {
            ushort count = 0;

            //ushort size = BitConverter.ToUInt16(_s.Array, _s.Offset);
            count += 2;
            //ushort id = BitConverter.ToUInt16(_s.Array, _s.Offset + 2);
            count += 2;
            this.playerId = BitConverter.ToInt64(new ReadOnlySpan<byte>(_s.Array, _s.Offset + count, _s.Count - count));
            count += 8;
        }

        public override ArraySegment<byte> Serialize()
        {
            ArraySegment<byte> s = SendBufferHelper.Open(4096);

            ushort count = 0;
            bool success = true;

            count += 2;
            success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset + count, s.Count - count), this.packedId);
            count += 2;
            success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset + count, s.Count - count), this.playerId);
            count += 8;
            success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset, s.Count), count);

            if (!success)
                return null;

            return SendBufferHelper.Close(count);
        }
    }

    public enum PacketID
    {
        PlayerInfoReq = 1,
        PlayerInfoRes = 2,
    }

    class ServerSession : Session
    {
        public override void OnConnected(EndPoint _endPoint)
        {
            Console.WriteLine($"OnConnected : {_endPoint}");

            PlayerInfoReq packet = new PlayerInfoReq() { playerId = 1001 };

            //for (int i = 0; i < 5; i++)
            {
                ArraySegment<byte> s = packet.Serialize();

                if (s != null)
                    Send(s);
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
