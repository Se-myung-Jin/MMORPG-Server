using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    class Session
    {
        Socket socket;

        public void Init(Socket socket)
        {
            this.socket = socket;
            SocketAsyncEventArgs recvArgs = new SocketAsyncEventArgs();
            recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);            
            recvArgs.SetBuffer(new byte[1024], 0, 1024);
        
            RecvMessage(recvArgs);
        }

        public void Send(byte[] sendBuff)
        {
            socket.Send(sendBuff);
        }

        public void Disconnect()
        {
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }

        void RecvMessage(SocketAsyncEventArgs args)
        {
            bool pending = socket.ReceiveAsync(args);
            if (false == pending)
                OnRecvCompleted(null, args);
        }

        void OnRecvCompleted(object sender, SocketAsyncEventArgs args)
        {
            // 0으로 올 때가 있다 (연결 끊었을 때)
            if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {
                try
                {
                    string recvData = Encoding.UTF8.GetString(args.Buffer, args.Offset, args.BytesTransferred);
                    Console.WriteLine($"[From Client] {recvData}");
                    RecvMessage(args);
                }
                catch (Exception ex)
                {

                    Console.WriteLine($"[Error] {ex.Message}");
                }
            }
            else
            {

            }
        }
    }
}
