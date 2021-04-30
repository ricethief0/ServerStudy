using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerCore
{
    public abstract class PacketSession : Session
    {
        public static readonly int HeaderSize = 2;
        //sealed 키워드 : cpp에서 상속시 접근제어에 private 으로 해놓은거나 마찬가지이다.
        // ex) class PacketSession : private Session{}
        public sealed override int OnReceive(ArraySegment<byte> buffers)
        {
            int processLen = 0;

            while(true)
            {
                //최소한 헤더는 파싱할수있는지 패킷사이즈로 확인
                if (buffers.Count < HeaderSize)
                    break;

                //패킷이 완전체로 도착했는지 확인 
                ushort dataSize = BitConverter.ToUInt16(buffers.Array, buffers.Offset);
                if (buffers.Count > dataSize) // 패킷사이즈가 실제 들어온 버퍼 사이즈보다 작으면 말이 안되니 브레이크
                    break;

                //패킷조립가능
                OnRecvPacket(buffers);

                processLen += dataSize;
                buffers = new ArraySegment<byte>(buffers.Array, buffers.Offset + dataSize, buffers.Count - dataSize);

            }

            return processLen;
        }

        public abstract void OnRecvPacket(ArraySegment<byte> buffers);
    }

        
    


    public abstract class Session
    {
        Socket m_socket;
        int mutex = 0;
        RecvBuffer m_recvBuffer = new RecvBuffer(1024);


        Queue<ArraySegment<byte>> sendQue = new Queue<ArraySegment<byte>>();
        List<ArraySegment<byte>> m_penddingList = new List<ArraySegment<byte>>();
        SocketAsyncEventArgs m_sendArgs = new SocketAsyncEventArgs();
        SocketAsyncEventArgs m_recArgs = new SocketAsyncEventArgs();
        object m_lock = new object();

        abstract public void OnConnected(EndPoint endPoint);
        abstract public void OnSend(int numOfBytes);
        abstract public int OnReceive(ArraySegment<byte> buffers);
        abstract public void OnDisConnected(EndPoint endPoint);

        public void Start(Socket socket)
        {
            m_socket = socket;
           
            m_recArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecevCompleted);
            m_sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompelted);
            RegisterRecev();
        }
        public void Send(ArraySegment<byte> buff)
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

            OnDisConnected(m_socket.RemoteEndPoint);
            m_socket.Shutdown(SocketShutdown.Both);
            m_socket.Close();
        }
        #region 네트워크통신

        void RegisterSend()
        {
            while(sendQue.Count>0)
            {
                ArraySegment<byte> buff = sendQue.Dequeue();
                m_penddingList.Add(buff);
            }
            m_sendArgs.BufferList = m_penddingList;

            bool pending = m_socket.SendAsync(m_sendArgs);
            if (pending == false)
            {
                OnSendCompelted(null, m_sendArgs);
            }
        }

        void OnSendCompelted(object obj, SocketAsyncEventArgs arg)
        {
            lock(m_lock)
            {
                if(arg.BytesTransferred>0 && SocketError.Success == arg.SocketError)
                {
                    try
                    {
                        m_sendArgs.BufferList = null;
                        m_penddingList.Clear();

                        OnSend(m_sendArgs.BytesTransferred);
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
            m_recvBuffer.Clean();
            ArraySegment<byte> sg = m_recvBuffer.WriteSegment;
            m_recArgs.SetBuffer(sg.Array,sg.Offset,sg.Count);

           bool pending =  m_socket.ReceiveAsync(m_recArgs);
            if(pending == false)
            {
                OnRecevCompleted(null, m_recArgs);
            }
        }

       
        void OnRecevCompleted(object sender, SocketAsyncEventArgs arg)
        {

            if (arg.BytesTransferred > 0 && SocketError.Success == arg.SocketError)
            {
                try
                {
                    // 쓰기에 버퍼 이동
                    if(!m_recvBuffer.OnWrite(arg.BytesTransferred)) 
                    {
                        DisConnect(); 
                        return; 
                    }
                    //컨텐츠 쪽으로 데이터를 넘겨주고 얼마나 처리했는지 받는다. 
                    int processLen = OnReceive(m_recvBuffer.ReadSegment);
                    if (processLen < 0 || processLen > m_recvBuffer.DataSize)
                    {
                        DisConnect();
                        return;
                    }

                    //읽기에 버퍼 이동
                    if (!m_recvBuffer.OnRead(processLen))
                    {
                        DisConnect();
                        return;
                    }
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
