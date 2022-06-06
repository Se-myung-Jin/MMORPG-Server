using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using ServerCore;

namespace Server
{
    class Program
    {
        static Listener listener = new Listener();

        static void Main(string[] _args)
        {
            // Domain Name System
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            listener.InitSocket(endPoint, () => { return new ClientSession(); });
            Console.WriteLine("Listening...");

            while (true)
            {
                ;
            }
        }
    }
}