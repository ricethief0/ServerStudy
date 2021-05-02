using System;
using System.Net;
using System.Net.Sockets;

namespace ServerCore
{
    // 리스너와 반대개념 ! (클라이언트에서 주로 썻던 아이)
    public class Connector // 필요한 이유 : 1. 서버도 분산처리를 해야할 때 다른 서버들과 통신하기 위해서는 필요하다. 즉, 서버끼리 통신하고자 할때도 필요하기 때문이다.
    {
        Func<Session> m_sessionFactory;

        public void Connect(IPEndPoint endPoint, Func<Session> sessionFactory, int count=1)
        {
            for(int i=0; i<count;i++)
            {
                Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                m_sessionFactory = sessionFactory;
                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.Completed += OnConnectCompleted;
                args.RemoteEndPoint = endPoint;
                args.UserToken = socket;

                RegisterConnect(args);
            }
            
        }

        void RegisterConnect(SocketAsyncEventArgs args)
        {
            Socket socket = args.UserToken as Socket;

            if (socket == null) return;

            bool pending = socket.ConnectAsync(args);
            if(pending == false)
            {
                OnConnectCompleted(null, args);
            }
        }

        void OnConnectCompleted(object sender, SocketAsyncEventArgs args)
        {
            if(args.SocketError == SocketError.Success)
            {
                Session session = m_sessionFactory.Invoke();
                session.Start(args.ConnectSocket); // 내가 연결한 소켓을 넘겨줌 , 유저토큰을 대신 사용해도 지장없지만 그냥 사용해봄.
                session.OnConnected(args.RemoteEndPoint);
            }
            else
            {
                Console.WriteLine($"OnConnectCompleted Fail : {args.SocketError}");
            }

        }


    }
}
