using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public class Connector
    {
        Func<Session> sessionFactory;

        public void Connect(IPEndPoint _endPoint, Func<Session> _sessionFactory)
        {
            Socket socket = new Socket(_endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += new EventHandler<SocketAsyncEventArgs>(OnConnectCompleted);
            args.RemoteEndPoint = _endPoint;
            args.UserToken = socket;

            sessionFactory = _sessionFactory;

            RegisterConnect(args);
        }

        void RegisterConnect(SocketAsyncEventArgs _args)
        {
            Socket socket = _args.UserToken as Socket;
            if (socket == null)
                return;

            bool pending = socket.ConnectAsync(_args);
            if (!pending)
                OnConnectCompleted(null, _args);
        }

        void OnConnectCompleted(object _sender, SocketAsyncEventArgs _args)
        {
            if (_args.SocketError == SocketError.Success)
            {
                Session session = sessionFactory.Invoke();
                session.Start(_args.ConnectSocket);
                session.OnConnected(_args.RemoteEndPoint);
            }
            else
            {
                Console.WriteLine($"OnConnectCompleted Fail : {_args.SocketError}");
            }
        }
    }
}
