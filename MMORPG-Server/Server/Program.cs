using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using ServerCore;

namespace Server
{
    public class Knight
    {
        public int hp;
        public int attack;
    }

    class GameSession : Session
    {
        public override void OnConnected(EndPoint _endPoint)
        {
            Console.WriteLine($"OnConnected : {_endPoint}");

            Knight knight = new Knight() { hp = 100, attack = 10 };

            ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);
            byte[] buffer = BitConverter.GetBytes(knight.hp);
            byte[] buffer2 = BitConverter.GetBytes(knight.attack);
            Array.Copy(buffer, 0, openSegment.Array, openSegment.Offset, buffer.Length);
            Array.Copy(buffer2, 0, openSegment.Array, openSegment.Offset + buffer.Length, buffer2.Length);
            ArraySegment<byte> sendBuffer = SendBufferHelper.Close(buffer.Length + buffer2.Length);
            Send(sendBuffer);

            Thread.Sleep(1000);

            Disconnect();
        }

        public override void OnDisconnected(EndPoint _endPoint)
        {
            Console.WriteLine($"OnDisconnected : {_endPoint}");
        }

        public override int OnRecv(ArraySegment<byte> _buffer)
        {
            string recvData = Encoding.UTF8.GetString(_buffer.Array, _buffer.Offset, _buffer.Count);

            Console.WriteLine($"[From Client] {recvData}");

            return _buffer.Count;
        }

        public override void OnSend(int _numOfBytes)
        {
            Console.WriteLine($"Transferred bytes: {_numOfBytes}");
        }
    }

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

            listener.InitSocket(endPoint, () => { return new GameSession(); });
            Console.WriteLine("Listening...");

            while (true)
            {
                ;
            }
        }
    }
}