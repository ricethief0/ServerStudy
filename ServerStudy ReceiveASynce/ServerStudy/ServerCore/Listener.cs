using System;
using System.Net;
using System.Net.Sockets;

namespace ServerCore
{
    public class Listener
    {
        static Socket m_listenSocket;
        Action<Socket> m_OnAcceptHandler;

        public void Init(IPEndPoint endPoint, Action<Socket> onAcceptHandler)
        {
            m_OnAcceptHandler += onAcceptHandler;

            m_listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            m_listenSocket.Bind(endPoint);
            m_listenSocket.Listen(10);

            SocketAsyncEventArgs arg = new SocketAsyncEventArgs();
            arg.Completed += new EventHandler<SocketAsyncEventArgs>(OnComplete);
            RegisterAccept(arg);
        }

        void OnComplete(object sender, SocketAsyncEventArgs arg)
        {
            if(SocketError.Success == arg.SocketError)
            {
                m_OnAcceptHandler.Invoke(arg.AcceptSocket);

            }
            else
            {
                Console.WriteLine($"socketError : {arg.SocketError.ToString()}"); 
            }
            RegisterAccept(arg);
        }

        void RegisterAccept(SocketAsyncEventArgs arg)
        {
            arg.AcceptSocket = null;

            bool pending = m_listenSocket.AcceptAsync(arg);
            if(pending == false)
            {
                OnComplete(null,arg);
            }
        }

    }
}
