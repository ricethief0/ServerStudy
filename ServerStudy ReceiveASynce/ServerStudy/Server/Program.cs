using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ServerCore;

namespace Server 
{
    
    class Program
    {
        static Listener listener = new Listener();
        public static GameRoom Room = new GameRoom();

        static void Main(string[] args)
        {
            

            //DNS(Domain Name System)
            string host = Dns.GetHostName(); // dns를 받아옴
            IPHostEntry hostEntry = Dns.GetHostEntry(host); // dns를 통해 ip를 받아옴
            IPAddress iPAddress = hostEntry.AddressList[0]; // ip리스트 중 첫번째 아이피를 받아옴
            IPEndPoint endPoint = new IPEndPoint(iPAddress, 7777); // 최종적으로 아이피와 포트번호를 저장

            listener.Init(endPoint, () => { return SessionManager.Instance.Create(); });
            Console.WriteLine("Waiting...");
            while (true)
            {; }




        }
    }
}
