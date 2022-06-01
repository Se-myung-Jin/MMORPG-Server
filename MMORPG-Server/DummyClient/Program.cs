using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DummyClient
{
    class Program
    {
        static void Main(string[] _args)
        {
            // Domain Name System
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                socket.Connect(endPoint);
                Console.WriteLine($"Connect To {socket.RemoteEndPoint.ToString()}");

                // send
                byte[] sendBuff = Encoding.UTF8.GetBytes("Hello World!");
                int sendBytes = socket.Send(sendBuff);

                // recv
                byte[] recvBuff = new byte[1024];
                int recvBytes = socket.Receive(recvBuff);
                string recvData = Encoding.UTF8.GetString(recvBuff, 0, recvBytes);
                Console.WriteLine($"[From Server] {recvData}");

                // close
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
            catch (Exception _ex)
            {
                Console.WriteLine(_ex.ToString());
            }
        }
    }
}