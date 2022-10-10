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

        static void Main(string[] args)
        {
            // DNS (Domain Name System)
            string host = Dns.GetHostName();                            // 로컬 DNS를 가져온다.
            IPHostEntry ipHost = Dns.GetHostEntry(host);                // 해당 IPHost를 가져온다.
            IPAddress ipAddr = ipHost.AddressList[0];                   // IP 주소를 가져온다.
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);         // 최종 IP 주소를 만들어준다.

            _listener.Init(endPoint, () => { return new ClientSession(); });
            Console.WriteLine("Listening..");

            while (true)
            {
                ;
            }
        }
    }
}