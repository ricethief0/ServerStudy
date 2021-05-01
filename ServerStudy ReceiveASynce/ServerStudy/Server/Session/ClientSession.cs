using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ServerCore;

namespace Server
{
   
    public class ClientSession : PacketSession
    {
        public int SeesionId { get; set; }
        public GameRoom Room { get; set; }
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");

            Program.Room.Enter(this);
            
        }

        public override void OnRecvPacket(ArraySegment<byte> buffers)
        {

            PacketManager.instance.OnRecvPacket(this, buffers);

        }

        public override void OnDisConnected(EndPoint endPoint)
        {
            SessionManager.Instance.Remove(this);
            if (Room != null)
            {
                Room.Leave(this);
                Room = null;
            }
            Console.WriteLine($"OnDisConnected : {endPoint}");
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Send Transferred bytes : {numOfBytes}");
        }
    }
}
