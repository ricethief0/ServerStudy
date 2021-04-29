using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ServerCore;

namespace DummyClient
{
    public class GameSession : Session
    {
        public override void OnConnected(EndPoint endPoint)
        {
            //Console.WriteLine($"OnConnected : {endPoint}");
            // 보내기 위함
            for (int i = 0; i < 5; i++)
            {
                Send(Encoding.UTF8.GetBytes($"I'm Client what are you name? {i.ToString("D2")}"));
            }
        }

        public override void OnDisConnected(EndPoint endPoint)
        {
            //Console.WriteLine($"OnDisConnected : {endPoint}");
        }

        public override void OnReceive(ArraySegment<byte> buffers)
        {
            string recData = Encoding.UTF8.GetString(buffers.Array, buffers.Offset, buffers.Count);

            Console.WriteLine($"[From Server] : {recData}");
        }


        public override void OnSend(int numOfBytes)
        {
            //Console.WriteLine($"Send Transferred bytes : {numOfBytes}");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            string host = Dns.GetHostName(); // dns를 받아옴
            IPHostEntry hostEntry = Dns.GetHostEntry(host); // dns를 통해 ip를 받아옴
            IPAddress iPAddress = hostEntry.AddressList[0]; // ip리스트 중 첫번째 아이피를 받아옴
            IPEndPoint endPoint = new IPEndPoint(iPAddress, 7777); // 최종적으로 아이피와 포트번호를 저장

            Connector connector = new Connector();

            connector.Connect(endPoint, () => { return new GameSession(); });


            while(true)
            {
                Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp); //  tcp형태의 소켓을 생성
                try
                {
                    
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
