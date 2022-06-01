using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerCore
{
    class Program
    {
        static Listener listener = new Listener();

        static void OnAcceptHandler(Socket _clientSocket)
        {
            try
            {
                // recv
                byte[] recvBuff = new byte[1024];
                int recvBytes = _clientSocket.Receive(recvBuff);
                string recvData = Encoding.UTF8.GetString(recvBuff, 0, recvBytes);
                Console.WriteLine($"[From Client] {recvData}");

                // send
                byte[] sendBuff = Encoding.UTF8.GetBytes("Welcome to MMORPG Server !");
                _clientSocket.Send(sendBuff);

                // 쫓아낸다
                _clientSocket.Shutdown(SocketShutdown.Both);
                _clientSocket.Close();
            }
            catch (Exception _ex)
            {
                Console.WriteLine(_ex);
            }
        }
        static void Main(string[] _args)
        {
            // Domain Name System
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            listener.InitSocket(endPoint, OnAcceptHandler);

            while (true)
            {
                ;
            }
        }
    }
}