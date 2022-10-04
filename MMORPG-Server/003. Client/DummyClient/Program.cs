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
            // DNS (Domain Name System)
            string host = Dns.GetHostName();                            // 로컬 DNS를 가져온다.
            IPHostEntry ipHost = Dns.GetHostEntry(host);                // 해당 IPHost를 가져온다.
            IPAddress ipAddr = ipHost.AddressList[0];                   // IP 주소를 가져온다.
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);         // 최종 IP 주소를 만들어준다.

            while (true)
            {
                Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    socket.Connect(endPoint);
                    Console.WriteLine($"Connect To {socket.RemoteEndPoint.ToString()}");

                    for (int i = 0; i < 5; i++)
                    {
                        byte[] sendBuff = Encoding.UTF8.GetBytes($"Hello World! {i}");
                        int sendBytes = socket.Send(sendBuff);
                    }

                    byte[] recvBuff = new byte[1024];
                    int recvBytes = socket.Receive(recvBuff);
                    string recvData = Encoding.UTF8.GetString(recvBuff, 0, recvBytes);
                    Console.WriteLine($"[From Server] {recvData}");

                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }

                Thread.Sleep(100);
            }
        }
    }
}