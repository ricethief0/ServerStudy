using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ServerCore;

namespace DummyClient
{
   
    public class ServerSession : Session
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");

            //C_Chat packet = new C_Chat() {playerId = 1001, name = "ABCD"};
            //packet.skills.Add(new C_PlayerInfoReq.Skill() { id = 101, level = 50, duration = 3.4f });
            //packet.skills.Add(new C_PlayerInfoReq.Skill() { id = 102, level = 30, duration = 2.4f });
            //packet.skills.Add(new C_PlayerInfoReq.Skill() { id = 103, level = 10, duration = 4.4f });
            //packet.skills.Add(new C_PlayerInfoReq.Skill() { id = 104, level = 5, duration = 1.4f });
            //// 보내기 위함
            ////for (int i = 0; i < 5; i++)
            //{
            //    ArraySegment<byte> sg = packet.Write();

            //    if (sg != null)
            //        Send(sg);

            //}
        }

        public override void OnDisConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisConnected : {endPoint}");
        }

        public override int OnReceive(ArraySegment<byte> buffers)
        {
            string recData = Encoding.UTF8.GetString(buffers.Array, buffers.Offset, buffers.Count);

            Console.WriteLine($"[From Server] : {recData}");
            return buffers.Count;
        }


        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Send Transferred bytes : {numOfBytes}");
        }
    }
}
