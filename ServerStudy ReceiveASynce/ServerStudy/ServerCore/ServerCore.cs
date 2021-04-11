using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerCore
{
    class Program
    {
        static Listener listener = new Listener();
        static void OnAcceptHandler(Socket clientSocket)
        {
            try
            {

                Session session = new Session();
                session.Start(clientSocket);
                byte[] buff = Encoding.UTF8.GetBytes("Welcome Client, My name is Server");
                session.Send(buff);

                Thread.Sleep(1000);
                session.DisConnect();
                
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex);
                
            }
        }

        static void Main(string[] args)
        {
            string host = Dns.GetHostName(); // dns를 받아옴
            IPHostEntry hostEntry = Dns.GetHostEntry(host); // dns를 통해 ip를 받아옴
            IPAddress iPAddress = hostEntry.AddressList[0]; // ip리스트 중 첫번째 아이피를 받아옴
            IPEndPoint endPoint = new IPEndPoint(iPAddress, 7777); // 최종적으로 아이피와 포트번호를 저장
            
            listener.Init(endPoint, OnAcceptHandler);
            Console.WriteLine("Waiting...");
            while(true)
            {; }
            



        }
    }
}
