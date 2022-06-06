using System.Net;
using ServerCore;

namespace Server
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

    class ClientSession : PacketSession
    {
        public override void OnConnected(EndPoint _endPoint)
        {
            Console.WriteLine($"OnConnected : {_endPoint}");

            //Packet packet = new Packet() { size = 100, packedId = 10 };

            //ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);
            //byte[] buffer = BitConverter.GetBytes(packet.size);
            //byte[] buffer2 = BitConverter.GetBytes(packet.packedId);
            //Array.Copy(buffer, 0, openSegment.Array, openSegment.Offset, buffer.Length);
            //Array.Copy(buffer2, 0, openSegment.Array, openSegment.Offset + buffer.Length, buffer2.Length);
            //ArraySegment<byte> sendBuffer = SendBufferHelper.Close(buffer.Length + buffer2.Length);


            //Send(sendBuffer);
            Thread.Sleep(5000);
            Disconnect();
        }

        public override void OnRecvPacket(ArraySegment<byte> _buffer)
        {
            ushort count = 0;

            ushort size = BitConverter.ToUInt16(_buffer.Array, _buffer.Offset);
            count += 2;
            ushort id = BitConverter.ToUInt16(_buffer.Array, _buffer.Offset + 2);
            count += 2;

            switch ((PacketID)id)
            {
                case PacketID.PlayerInfoReq:
                    {
                        PlayerInfoReq playerInfoReq = new PlayerInfoReq();
                        playerInfoReq.Deserialize(_buffer);

                        Console.WriteLine($"PlayerInfoReq: {playerInfoReq.playerId}");
                    }
                    break;

            }

            Console.WriteLine($"RecvPacketId {id}, Size {size}");
        }

        public override void OnDisconnected(EndPoint _endPoint)
        {
            Console.WriteLine($"OnDisconnected : {_endPoint}");
        }

        public override void OnSend(int _numOfBytes)
        {
            Console.WriteLine($"Transferred bytes: {_numOfBytes}");
        }
    }
}
