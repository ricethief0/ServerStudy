using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerCore
{
    public class GameSession : Session
    {
        public override void OnConnected(EndPoint endPoint)
        {
            //Console.WriteLine($"OnConnected : {endPoint}");
            
            byte[] buff = Encoding.UTF8.GetBytes("Welcome Client, My name is Server");
            Send(buff);
            Thread.Sleep(1000);
            DisConnect();
        }

        public override void OnDisConnected(EndPoint endPoint)
        {
            //Console.WriteLine($"OnDisConnected : {endPoint}");
        }

        public override void OnReceive(ArraySegment<byte> buffers)
        {
            string recData = Encoding.UTF8.GetString(buffers.Array, buffers.Offset, buffers.Count);

            Console.WriteLine($"[Client Data] : {recData}");
        }


        public override void OnSend(int numOfBytes)
        {
            //Console.WriteLine($"Send Transferred bytes : {numOfBytes}");
        }
    }
    class Program
    {
        static Listener listener = new Listener();
        

        static void Main(string[] args)
        {
            string host = Dns.GetHostName(); // dns를 받아옴
            IPHostEntry hostEntry = Dns.GetHostEntry(host); // dns를 통해 ip를 받아옴
            IPAddress iPAddress = hostEntry.AddressList[0]; // ip리스트 중 첫번째 아이피를 받아옴
            IPEndPoint endPoint = new IPEndPoint(iPAddress, 7777); // 최종적으로 아이피와 포트번호를 저장

            listener.Init(endPoint, () => { return new GameSession(); });
            Console.WriteLine("Waiting...");
            while(true)
            {; }
            



        }
    }
}
