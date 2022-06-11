using System.Text;
using ServerCore;
using System.Net;

namespace DummyClient
{
    class ServerSession : Session
    {
        public override void OnConnected(EndPoint _endPoint)
        {
            Console.WriteLine($"OnConnected : {_endPoint}");

            PlayerInfoReq packet = new PlayerInfoReq() { playerId = 1001, name = "Garden" };
            packet.skills.Add(new PlayerInfoReq.Skill() { id = 1, level = 1, duration = 0.15f });
            packet.skills.Add(new PlayerInfoReq.Skill() { id = 2, level = 2, duration = 0.25f });
            packet.skills.Add(new PlayerInfoReq.Skill() { id = 3, level = 3, duration = 0.35f });
            packet.skills.Add(new PlayerInfoReq.Skill() { id = 4, level = 4, duration = 0.45f });

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
