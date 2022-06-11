using System.Net;
using ServerCore;
using System.Text;

namespace Server
{
    class PlayerInfoReq
    {
        public long playerId;
        public string name;

        public struct SkillInfo
        {
            public int id;
            public short level;
            public float duration;

            public bool Write(Span<byte> _s, ref ushort _count)
            {
                bool success = true;
                success &= BitConverter.TryWriteBytes(_s.Slice(_count, _s.Length - _count), id);
                _count += sizeof(int);
                success &= BitConverter.TryWriteBytes(_s.Slice(_count, _s.Length - _count), level);
                _count += sizeof(short);
                success &= BitConverter.TryWriteBytes(_s.Slice(_count, _s.Length - _count), duration);
                _count += sizeof(float);

                return true;
            }

            public void Read(ReadOnlySpan<byte> _s, ref ushort _count)
            {
                id = BitConverter.ToInt32(_s.Slice(_count, _s.Length - _count));
                _count += sizeof(int);
                level = BitConverter.ToInt16(_s.Slice(_count, _s.Length - _count));
                _count += sizeof(short);
                duration = BitConverter.ToSingle(_s.Slice(_count, _s.Length - _count));
                _count += sizeof(float);
            }
        }

        public List<SkillInfo> skills = new List<SkillInfo>();

        public void Deserialize(ArraySegment<byte> _segment)
        {
            ushort count = 0;

            ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(_segment.Array, _segment.Offset, _segment.Count);

            count += sizeof(ushort);
            count += sizeof(ushort);
            this.playerId = BitConverter.ToInt64(s.Slice(count, s.Length - count));
            count += sizeof(long);

            // string
            ushort nameLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
            count += sizeof(ushort);
            this.name = Encoding.Unicode.GetString(s.Slice(count, nameLen));
            count += nameLen;

            // skill list
            skills.Clear();
            ushort skillLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
            count += sizeof(ushort);
            for (int i = 0; i < skillLen; i++)
            {
                SkillInfo skill = new SkillInfo();
                skill.Read(s, ref count);
                skills.Add(skill);
            }
        }

        public ArraySegment<byte> Serialize()
        {
            ArraySegment<byte> segment = SendBufferHelper.Open(4096);

            ushort count = 0;
            bool success = true;

            Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

            count += sizeof(ushort);
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.PlayerInfoReq);
            count += sizeof(ushort);
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.playerId);
            count += sizeof(long);

            // string
            ushort nameLen = (ushort)Encoding.Unicode.GetBytes(this.name, 0, this.name.Length, segment.Array, segment.Offset + count + sizeof(ushort));
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), nameLen);
            count += sizeof(ushort);
            count += nameLen;

            // skill list
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)skills.Count);
            count += sizeof(ushort);
            foreach (SkillInfo skill in skills)
            {
                success &= skill.Write(s, ref count);
            }

            success &= BitConverter.TryWriteBytes(s, count);

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

                        Console.WriteLine($"PlayerInfoReq: {playerInfoReq.playerId}, {playerInfoReq.name}");

                        foreach (PlayerInfoReq.SkillInfo skill in playerInfoReq.skills)
                        {
                            Console.WriteLine($"Skill({skill.id})({skill.level})({skill.duration})");
                        }
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
