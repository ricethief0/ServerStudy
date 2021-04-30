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
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");

            //Packet packet = new Packet() { size = 100, packetId = 10 };

            //ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);

            //byte[] buffer1 = BitConverter.GetBytes(packet.size);
            //byte[] buffer2 = BitConverter.GetBytes(packet.packetId);

            //Array.Copy(buffer1, 0, openSegment.Array, openSegment.Offset, buffer1.Length);
            //Array.Copy(buffer2, 0, openSegment.Array, openSegment.Offset+buffer1.Length, buffer2.Length);
            //ArraySegment<byte> sendBuff = SendBufferHelper.Close(buffer1.Length + buffer2.Length);

            //Send(sendBuff);
            Thread.Sleep(5000);
            DisConnect();
        }

        public override void OnRecvPacket(ArraySegment<byte> buffers)
        {
            ushort count = 0;
            ushort size = BitConverter.ToUInt16(buffers.Array, buffers.Offset);
            count += 2;
            ushort id = BitConverter.ToUInt16(buffers.Array, buffers.Offset + count);
            count += 2;

            switch((PacketID)id)
            {
                case PacketID.PlayerInfoReq:
                    {
                        PlayerInfoReq req = new PlayerInfoReq();
                        req.Read(buffers);
                        Console.WriteLine($"PlayerId:{req.playerId} / name : {req.name}");

                        foreach(PlayerInfoReq.Skill skill in req.skills)
                        {
                            Console.WriteLine($"skill id : {skill.id} / level : {skill.level} / duration : {skill.duration}");
                        }
                    }
                    break;
           
           
                
            }
            Console.WriteLine($"size : {size} / id : {id}");

        }

        public override void OnDisConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisConnected : {endPoint}");
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Send Transferred bytes : {numOfBytes}");
        }
    }
}
