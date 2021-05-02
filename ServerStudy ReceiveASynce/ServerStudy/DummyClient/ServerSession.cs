using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ServerCore;

namespace DummyClient
{
   
    public class ServerSession : PacketSession
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");

        }

        public override void OnDisConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisConnected : {endPoint}");
        }

      

        public override void OnRecvPacket(ArraySegment<byte> buffers)
        {
            PacketManager.Instance.OnRecvPacket(this, buffers);
        }

        public override void OnSend(int numOfBytes)
        {
            //Console.WriteLine($"Send Transferred bytes : {numOfBytes}");
        }
    }
}
