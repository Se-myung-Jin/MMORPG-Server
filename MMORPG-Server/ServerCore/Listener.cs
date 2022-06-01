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

        Func<Session> sessionFactory;

        public void InitSocket(IPEndPoint _endPoint, Func<Session> _sessionFactory)
        {
            listenSocket = new Socket(_endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            listenSocket.Bind(_endPoint);

            listenSocket.Listen(10);

            // Accept 완료 처리
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
            RegisterAccept(args);

            sessionFactory += _sessionFactory;
        }

        void RegisterAccept(SocketAsyncEventArgs _args)
        {
            // 이전 AcceptSocket의 정보를 밀어줘야 한다
            _args.AcceptSocket = null;

            bool pending = listenSocket.AcceptAsync(_args);
            if (!pending) // 비동기로 던졌지만 바로 받았을 경우에만 호출
                OnAcceptCompleted(null, _args);
        }

        void OnAcceptCompleted(object _sender, SocketAsyncEventArgs _args)
        {
            if (_args.SocketError == SocketError.Success)
            {
                Session session = sessionFactory.Invoke();
                session.Start(_args.AcceptSocket);
                session.OnConnected(_args.AcceptSocket.RemoteEndPoint);
            }
            else
            {
                Console.WriteLine(_args.SocketError.ToString());
            }

            RegisterAccept(_args);
        }
    }
}
