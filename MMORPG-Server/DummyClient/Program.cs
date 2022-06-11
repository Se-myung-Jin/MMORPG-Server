using ServerCore;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DummyClient
{
    class Program
    {
        static void Main(string[] args)
        {
            // Domain Name System
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            Connector connector = new Connector();

            connector.Connect(endPoint, () => { return new ServerSession(); });

            while (true)
            {
                try
                {

                }
                catch (Exception _ex)
                {
                    Console.WriteLine(_ex.ToString());
                }

                Thread.Sleep(100);
            }
        }
    }
}