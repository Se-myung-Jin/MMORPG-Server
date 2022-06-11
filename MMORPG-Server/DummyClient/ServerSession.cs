using System.Text;
using ServerCore;
using System.Net;

namespace DummyClient
{
    class ServerSession : Session
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");

            C_PlayerInfoReq packet = new C_PlayerInfoReq() { playerId = 1001, name = "Garden" };
            packet.skills.Add(new C_PlayerInfoReq.Skill() { id = 1, level = 1, duration = 0.15f });
            packet.skills.Add(new C_PlayerInfoReq.Skill() { id = 2, level = 2, duration = 0.25f });
            packet.skills.Add(new C_PlayerInfoReq.Skill() { id = 3, level = 3, duration = 0.35f });
            packet.skills.Add(new C_PlayerInfoReq.Skill() { id = 4, level = 4, duration = 0.45f });

            //for (int i = 0; i < 5; i++)
            {
                ArraySegment<byte> s = packet.Serialize();

                if (s != null)
                    Send(s);
            }
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected : {endPoint}");
        }

        public override int OnRecv(ArraySegment<byte> buffer)
        {
            string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);

            Console.WriteLine($"[From Server] {recvData}");

            return buffer.Count;
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Transferred bytes: {numOfBytes}");
        }
    }
}
