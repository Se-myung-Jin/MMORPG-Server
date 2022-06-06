using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public abstract class PacketSession : Session
    {
        public static readonly int HeaderSize = 2;

        // [size(2)][packetID(2)][...]
        public sealed override int OnRecv(ArraySegment<byte> _buffer)
        {
            int processLen = 0;

            while (true)
            {
                // 최소한 헤더는 파싱할 수 있는지 확인
                if (_buffer.Count < HeaderSize)
                    break;

                // 패킷이 완전체로 도착했는지 확인
                ushort dataSize = BitConverter.ToUInt16(_buffer.Array, _buffer.Offset);
                if (_buffer.Count < dataSize)
                    break;

                // 여기까지 왔으면 패킷 조립 가능
                OnRecvPacket(new ArraySegment<byte>(_buffer.Array, _buffer.Offset, dataSize));

                processLen += dataSize;
                _buffer = new ArraySegment<byte>(_buffer.Array, _buffer.Offset + dataSize, _buffer.Count - dataSize);
            }

            return processLen;
        }

        public abstract void OnRecvPacket(ArraySegment<byte> _buffer);
    }

    public abstract class Session
    {
        Socket socket;

        int disconnected = 0;

        RecvBuffer recvBuffer = new RecvBuffer(1024);

        SocketAsyncEventArgs recvArgs = new SocketAsyncEventArgs();
        SocketAsyncEventArgs sendArgs = new SocketAsyncEventArgs();

        Queue<ArraySegment<byte>> sendQueue = new Queue<ArraySegment<byte>>();

        List<ArraySegment<byte>> pendingList = new List<ArraySegment<byte>>();

        object sendLock = new object();

        public abstract void OnConnected(EndPoint _endPoint);
        public abstract int OnRecv(ArraySegment<byte> _buffer);
        public abstract void OnSend(int _numOfBytes);
        public abstract void OnDisconnected(EndPoint _endPoint);

        public void Start(Socket _socket)
        {
            socket = _socket;

            recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);
            sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);

            RegisterRecv();
        }

        public void Send(ArraySegment<byte> _sendBuff)
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
                ArraySegment<byte> buff = sendQueue.Dequeue();
                pendingList.Add(buff);
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
            recvBuffer.Clean();
            ArraySegment<byte> segment = recvBuffer.WriteSegment;
            recvArgs.SetBuffer(segment.Array, segment.Offset, segment.Count);

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
                    // WritePos 이동
                    if (recvBuffer.OnWrite(_args.BytesTransferred) == false)
                    {
                        Disconnect();
                        return;
                    }

                    // 컨텐츠 쪽으로 넘겨주고 얼마나 처리했는지 받는다
                    int processLen = OnRecv(recvBuffer.ReadSegment);
                    if (processLen < 0 || recvBuffer.DataSize < processLen)
                    {
                        Disconnect();
                        return;
                    }

                    // ReadPos 이동
                    if (recvBuffer.OnRead(processLen) == false)
                    {
                        Disconnect();
                        return;
                    }

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
