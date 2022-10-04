using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using static System.Collections.Specialized.BitVector32;

namespace ServerCore
{
    class GameSession : Session
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");
            
            byte[] sendBuff = Encoding.UTF8.GetBytes("Welcome to MMORPG Server !");
            Send(sendBuff);
            Thread.Sleep(1000);
            Disconnect();
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected : {endPoint}");
        }

        public override void OnRecv(ArraySegment<byte> buffer)
        {
            string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
            Console.WriteLine($"[From Client] {recvData}");
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Transferred bytes: {numOfBytes}");
        }
    }
    class Program
    {
        static Listener _listener = new Listener();

        static void Main(string[] args)
        {
            // DNS (Domain Name System)
            string host = Dns.GetHostName();                            // 로컬 DNS를 가져온다.
            IPHostEntry ipHost = Dns.GetHostEntry(host);                // 해당 IPHost를 가져온다.
            IPAddress ipAddr = ipHost.AddressList[0];                   // IP 주소를 가져온다.
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);         // 최종 IP 주소를 만들어준다.

            _listener.Init(endPoint, () => { return new GameSession(); });
            Console.WriteLine("Listening..");

            while (true)
            {
                ;
            }
        }
    }
}