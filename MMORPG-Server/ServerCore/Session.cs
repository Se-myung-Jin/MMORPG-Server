using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    abstract class Session
    {
        Socket socket;

        int disconnected = 0;

        SocketAsyncEventArgs recvArgs = new SocketAsyncEventArgs();
        SocketAsyncEventArgs sendArgs = new SocketAsyncEventArgs();

        Queue<byte[]> sendQueue = new Queue<byte[]>();

        List<ArraySegment<byte>> pendingList = new List<ArraySegment<byte>>();

        object sendLock = new object();

        public abstract void OnConnected(EndPoint _endPoint);
        public abstract void OnRecv(ArraySegment<byte> _buffer);
        public abstract void OnSend(int _numOfBytes);
        public abstract void OnDisconnected(EndPoint _endPoint);

        public void Start(Socket _socket)
        {
            socket = _socket;

            recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);
            recvArgs.SetBuffer(new byte[1024], 0, 1024);

            sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);

            RegisterRecv();
        }

        public void Send(byte[] _sendBuff)
        {
            lock (sendLock)
            {
                sendQueue.Enqueue(_sendBuff);
                if (pendingList.Count == 0)
                    RegisterSend();
            }
        }

        public void Disconnect()
        {
            if (Interlocked.Exchange(ref disconnected, 1) == 1)
                return;

            OnDisconnected(socket.RemoteEndPoint);
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }

        #region 네트워크 통신

        void RegisterSend()
        {
            while (sendQueue.Count > 0)
            {
                byte[] buff = sendQueue.Dequeue();
                pendingList.Add(new ArraySegment<byte>(buff, 0, buff.Length));
            }
            sendArgs.BufferList = pendingList;

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
                        sendArgs.BufferList = null;
                        pendingList.Clear();

                        OnSend(_args.BytesTransferred);

                        if (sendQueue.Count > 0)
                            RegisterSend();
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

        void RegisterRecv()
        {
            bool pending = socket.ReceiveAsync(recvArgs);
            if (!pending)
                OnRecvCompleted(null, recvArgs);
        }

        void OnRecvCompleted(object _sender, SocketAsyncEventArgs _args)
        {
            if (_args.BytesTransferred > 0 && _args.SocketError == SocketError.Success)
            {
                try
                {
                    OnRecv(new ArraySegment<byte>(_args.Buffer, _args.Offset, _args.BytesTransferred));

                    RegisterRecv();
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
