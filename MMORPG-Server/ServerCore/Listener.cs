using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    class Listener
    {
        Socket listenSocket;

        Action<Socket> onAcceptHandler;

        public void InitSocket(IPEndPoint _endPoint, Action<Socket> _onAcceptHandler)
        {
            listenSocket = new Socket(_endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            listenSocket.Bind(_endPoint);

            listenSocket.Listen(10);

            // Accept 완료 처리
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
            RegisterAccept(args);

            onAcceptHandler += _onAcceptHandler;
        }

        void RegisterAccept(SocketAsyncEventArgs _args)
        {
            _args.AcceptSocket = null;

            bool pending = listenSocket.AcceptAsync(_args);
            if (!pending) // 비동기로 던졌지만 바로 받았을 경우에만 호출
                OnAcceptCompleted(null, _args);
        }

        void OnAcceptCompleted(object _sender, SocketAsyncEventArgs _args)
        {
            if (_args.SocketError == SocketError.Success)
            {
                onAcceptHandler.Invoke(_args.AcceptSocket);
            }
            else
            {
                Console.WriteLine(_args.SocketError.ToString());
            }

            RegisterAccept(_args);
        }
    }
}
