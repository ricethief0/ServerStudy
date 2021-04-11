using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace DummyClient
{
    class Program
    {
        static void Main(string[] args)
        {
            string host = Dns.GetHostName(); // dns를 받아옴
            IPHostEntry hostEntry = Dns.GetHostEntry(host); // dns를 통해 ip를 받아옴
            IPAddress iPAddress = hostEntry.AddressList[0]; // ip리스트 중 첫번째 아이피를 받아옴
            IPEndPoint endPoint = new IPEndPoint(iPAddress, 7777); // 최종적으로 아이피와 포트번호를 저장




            while(true)
            {
                Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp); //  tcp형태의 소켓을 생성
                try
                {
                    socket.Connect(endPoint);
                    Console.WriteLine($"connected : {socket.RemoteEndPoint.ToString()}");
                    for(int i=0;i<5; i++)
                    {
                        socket.Send(Encoding.UTF8.GetBytes($"I'm Client what are you name? {i.ToString("D2")}"));

                    }

                    byte[] serverBuff = new byte[1024];
                    int buffSize = socket.Receive(serverBuff);

                    Console.WriteLine($"[Server] : {Encoding.UTF8.GetString(serverBuff, 0, buffSize)}");

                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

                Thread.Sleep(100);              
            }
          
            
        }
    
    }
}
