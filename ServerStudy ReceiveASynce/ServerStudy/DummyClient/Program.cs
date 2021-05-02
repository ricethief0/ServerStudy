using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ServerCore;

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

            Connector connector = new Connector();

            connector.Connect(endPoint, () => { return SessionManager.Instance.Generate(); },100); // 클라이언트 접속자를 만들수있음.


            while(true)
            {
                Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp); //  tcp형태의 소켓을 생성
                try
                {
                    SessionManager.Instance.SendForEach();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

                Thread.Sleep(250);           //1초에 4번 보내기위해 만든것
                
            }
          
            
        }
    
    }
}
