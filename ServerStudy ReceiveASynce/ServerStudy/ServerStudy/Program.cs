using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ServerCore;

namespace Server 
{
    public class Kight
    {
        public int m_hp;
        public int m_atk;
    }
    public class GameSession : Session
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");

            Kight kight = new Kight() { m_hp = 100, m_atk = 10 };

            ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);

            byte[] buffer1 = BitConverter.GetBytes(kight.m_atk);
            byte[] buffer2 = BitConverter.GetBytes(kight.m_hp);

            Array.Copy(buffer1, 0, openSegment.Array, openSegment.Offset, buffer1.Length);
            Array.Copy(buffer2, 0, openSegment.Array, openSegment.Offset+buffer1.Length, buffer2.Length);
            ArraySegment<byte> sendbuufer = SendBufferHelper.Close(buffer1.Length + buffer2.Length);

            Send(sendbuufer);
            Thread.Sleep(1000);
            DisConnect();
        }

        public override void OnDisConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisConnected : {endPoint}");
        }

        public override int OnReceive(ArraySegment<byte> buffers)
        {
            string recData = Encoding.UTF8.GetString(buffers.Array, buffers.Offset, buffers.Count);

            Console.WriteLine($"[Client Data] : {recData}");
            return buffers.Count;
        }


        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Send Transferred bytes : {numOfBytes}");
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
            while (true)
            {; }




        }
    }
}
