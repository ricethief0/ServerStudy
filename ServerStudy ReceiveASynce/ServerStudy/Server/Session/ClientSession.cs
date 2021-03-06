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

            Program.Room.Push(() => Program.Room.Enter(this));
        }

        public override void OnRecvPacket(ArraySegment<byte> buffers)
        {

            PacketManager.Instance.OnRecvPacket(this, buffers);

        }

        public override void OnDisConnected(EndPoint endPoint)
        {
            SessionManager.Instance.Remove(this);
            if (Room != null)
            {
                GameRoom room = Room; // 이렇게 함으로 인해 잡큐에 작업자가 Room이 null이 되도 room에 있는 정보로 보기때문에 문제 되지않음.
                room.Push(() => room.Leave(this));
                Room = null;
            }
            Console.WriteLine($"OnDisConnected : {endPoint}");
        }

      

        public override void OnSend(int numOfBytes)
        {
            //Console.WriteLine($"Send Transferred bytes : {numOfBytes}");
        }
    }
}
