using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using ServerCore;

namespace Server
{
    class Program
    {
        static Listener _listener = new Listener();
        public static GameRoom Room = new GameRoom();

        static void Main(string[] args)
        {
            // Domain Name System
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            _listener.InitSocket(endPoint, () => { return SessionManager.Instance.Generate(); });
            Console.WriteLine("Listening...");

            while (true)
            {
                Room.Flush();
                Thread.Sleep(250);
            }
        }
    }
}