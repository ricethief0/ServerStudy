using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerCore
{
    
    public class Listener
    {
        static Socket m_listenSocket;
        Func<Session> m_sessionFactory;

        public void Init(IPEndPoint endPoint, Func<Session> sessionFactory)
        {
            m_sessionFactory += sessionFactory;

            m_listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            m_listenSocket.Bind(endPoint);
            m_listenSocket.Listen(10);

            SocketAsyncEventArgs arg = new SocketAsyncEventArgs();
            arg.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
            RegisterAccept(arg);
        }
        void RegisterAccept(SocketAsyncEventArgs arg)
        {
            arg.AcceptSocket = null;

            bool pending = m_listenSocket.AcceptAsync(arg);
            if (pending == false)
            {
                OnAcceptCompleted(null, arg);
            }
        }
        void OnAcceptCompleted(object sender, SocketAsyncEventArgs arg)
        {
            if(SocketError.Success == arg.SocketError)
            {
                Session session = m_sessionFactory.Invoke();
                session.Start(arg.AcceptSocket);
                session.OnConnected(arg.RemoteEndPoint);
                
            }
            else
            {
                Console.WriteLine($"socketError : {arg.SocketError.ToString()}"); 
            }
            RegisterAccept(arg);
        }

       

    }
}
