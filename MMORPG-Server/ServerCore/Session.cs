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

        int disconnected = 0;

        SocketAsyncEventArgs sendArgs = new SocketAsyncEventArgs();

        Queue<byte[]> sendQueue = new Queue<byte[]>();

        bool queuePending = false;

        object sendLock = new object();

        public void Start(Socket _socket)
        {
            socket = _socket;

            SocketAsyncEventArgs recvArgs = new SocketAsyncEventArgs();
            recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);
            recvArgs.SetBuffer(new byte[1024], 0, 1024);

            sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);

            RegisterRecv(recvArgs);
        }

        public void Send(byte[] _sendBuff)
        {
            lock (sendLock)
            {
                sendQueue.Enqueue(_sendBuff);
                if (!queuePending)
                    RegisterSend();
            }
        }

        public void Disconnect()
        {
            if (Interlocked.Exchange(ref disconnected, 1) == 1)
                return;

            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }

        #region 네트워크 통신

        void RegisterSend()
        {
            queuePending = true;

            byte[] buff = sendQueue.Dequeue();
            sendArgs.SetBuffer(buff, 0, buff.Length);

            bool pending = socket.SendAsync(sendArgs);
            if (!pending)
                OnSendCompleted(null, sendArgs);
        }

        void OnSendCompleted(object _sender, SocketAsyncEventArgs _args)
        {
            lock (sendLock)
            {
                if (_args.BytesTransferred > 0 && _args.SocketError == SocketError.Success)
                {
                    try
                    {
                        if (sendQueue.Count > 0)
                            RegisterSend();
                        else
                            queuePending = false;
                    }
                    catch (Exception _ex)
                    {
                        Console.WriteLine(_ex);
                    }
                }
                else
                {
                    Disconnect();
                }
            }
        }

        void RegisterRecv(SocketAsyncEventArgs _args)
        {
            bool pending = socket.ReceiveAsync(_args);
            if (!pending)
                OnRecvCompleted(null, _args);
        }

        void OnRecvCompleted(object _sender, SocketAsyncEventArgs _args)
        {
            if (_args.BytesTransferred > 0 && _args.SocketError == SocketError.Success)
            {
                try
                {
                    string recvData = Encoding.UTF8.GetString(_args.Buffer, _args.Offset, _args.BytesTransferred);

                    Console.WriteLine($"[From Client] {recvData}");

                    RegisterRecv(_args);
                }
                catch (Exception _ex)
                {
                    Console.WriteLine(_ex);
                }
            }
            else
            {
                Disconnect();
            }
        }

        #endregion 네트워크 통신
    }
}
