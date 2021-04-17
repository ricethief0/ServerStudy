﻿using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerCore
{
    public class Session
    {
        Socket m_socket;
        int mutex = 0;
        Queue<byte[]> sendQue = new Queue<byte[]>();
        //bool m_isPending = false;
        List<ArraySegment<byte>> m_penddingList = new List<ArraySegment<byte>>();
        SocketAsyncEventArgs m_sendArgs = new SocketAsyncEventArgs();
        SocketAsyncEventArgs m_recArgs = new SocketAsyncEventArgs();
        object m_lock = new object();
        public void Start(Socket socket)
        {
            m_socket = socket;

           
            m_recArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecevComplete);
            m_recArgs.SetBuffer(new byte[1024], 0, 1024);
            RegisterRecev();

            m_sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompelte);

        }
        public void Send(byte[] buff)
        {
            lock(m_lock)
            {
                sendQue.Enqueue(buff);
                if (m_penddingList.Count == 0)
                {
                    RegisterSend();
                }
            }
            
        }

        public void DisConnect()
        {
            if (Interlocked.Exchange(ref mutex, 1) == 1)
                return;

            m_socket.Shutdown(SocketShutdown.Both);
            m_socket.Close();
        }
        #region 네트워크통신

        void RegisterSend()
        {
            while(sendQue.Count>0)
            {
                byte[] buff = sendQue.Dequeue();
                m_penddingList.Add(new ArraySegment<byte>(buff, 0, buff.Length));
            }
            m_sendArgs.BufferList = m_penddingList;

            bool pending = m_socket.SendAsync(m_sendArgs);
            if (pending == false)
            {
                OnSendCompelte(null, m_sendArgs);
            }
        }

        void OnSendCompelte(object obj, SocketAsyncEventArgs arg)
        {
            lock(m_lock)
            {
                if(arg.BytesTransferred>0 && SocketError.Success == arg.SocketError)
                {
                    try
                    {
                        m_sendArgs.BufferList = null;
                        m_penddingList.Clear();
                        if (sendQue.Count > 0)
                        {
                            RegisterSend();
                        }
                    }
                    catch (Exception e )
                    {

                        Console.WriteLine($"Exception : {e}");
                    }
                }
                else
                {
                    DisConnect();
                }
            }
        }


        void RegisterRecev()
        {
           bool pending =  m_socket.ReceiveAsync(m_recArgs);
            if(pending == false)
            {
                OnRecevComplete(null, m_recArgs);
            }
        }

       
        void OnRecevComplete(object sender, SocketAsyncEventArgs arg)
        {

            if (arg.BytesTransferred > 0 && SocketError.Success == arg.SocketError)
            {
                try
                {
                    string recData = Encoding.UTF8.GetString(arg.Buffer, arg.Offset, arg.BytesTransferred);

                    Console.WriteLine($"[Client Data] : {recData}");
                    RegisterRecev();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"OnRecvComplete Fail : {ex}");
                }
            }
            else
            {
                DisConnect();
            }
        }
        #endregion
    }
}
