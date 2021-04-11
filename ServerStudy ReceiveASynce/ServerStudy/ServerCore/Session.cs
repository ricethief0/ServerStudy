using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerCore
{
    public class Session
    {
        Socket m_socket;
        int mutex = 0;

        public void Start(Socket socket)
        {
            m_socket = socket;
            SocketAsyncEventArgs arg = new SocketAsyncEventArgs();
            arg.Completed += new EventHandler<SocketAsyncEventArgs>(OnComplete);

            arg.SetBuffer(new byte[1024], 0, 1024);
            RegisterRecev(arg);
        }
        public void Send(byte[] buff)
        {
            m_socket.Send(buff);
        }

        public void DisConnect()
        {
            if (Interlocked.Exchange(ref mutex, 1) == 1)
                return;

            m_socket.Shutdown(SocketShutdown.Both);
            m_socket.Close();
        }
        #region 네트워크통신
        void RegisterRecev(SocketAsyncEventArgs arg)
        {
           bool pending =  m_socket.ReceiveAsync(arg);
            if(pending == false)
            {
                OnComplete(null, arg);
            }
            

        }

       
        void OnComplete(object sender, SocketAsyncEventArgs arg)
        {
            try
            {
                if (arg.BytesTransferred > 0 && SocketError.Success == arg.SocketError)
                {
                    string recData = Encoding.UTF8.GetString(arg.Buffer, arg.Offset, arg.BytesTransferred);

                    Console.WriteLine($"[Client Data] : {recData}");
                    RegisterRecev(arg);
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"OnRecvComplete Fail : {ex}");
            }
            
        }
        #endregion
    }
}
