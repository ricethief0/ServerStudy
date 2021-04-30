using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ServerCore;

namespace DummyClient
{
   

    class PlayerInfoReq  //body
    {
        public long playerId;   //자료형 고정 
        public string name;     //고정되지 않은 자료형

        public struct Skill
        {
            public int id;
            public short level;
            public float duration;

            public bool Write(Span<byte> span, ref ushort count)
            {
                bool success = true;
                success &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count),id);
                count += sizeof(int);
                success &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), level);
                count += sizeof(short);
                success &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), duration);
                count += sizeof(float);

                return success;
            }

            public void Read(ReadOnlySpan<byte> span,ref ushort count)
            {
                id = BitConverter.ToInt32(span.Slice(count,span.Length-count));
                count += sizeof(int);
                level = BitConverter.ToInt16(span.Slice(count, span.Length - count));
                count += sizeof(short);
                duration = BitConverter.ToSingle(span.Slice(count, span.Length - count)); // float은 이름이 single로 되어있고 double은 이름이 같은걸로 존재함
                count += sizeof(float);
            }
        }

        public List<Skill> skills = new List<Skill>();

        

        public  void Read(ArraySegment<byte> sg)
        {

            ushort count = 0;

            ReadOnlySpan<byte> span = new Span<byte>(sg.Array,sg.Offset,sg.Count);

            //ushort size = BitConverter.ToUInt16(sg.Array, sg.Offset);
            count += sizeof(ushort);
            //ushort id = BitConverter.ToUInt16(sg.Array, sg.Offset + count);
            count += sizeof(ushort);
            this.playerId = BitConverter.ToInt64(span.Slice(count,span.Length-count));
            count += sizeof(long);

            //string 같은 가변적인 데이터 넣기
            ushort nameLen = BitConverter.ToUInt16(span.Slice(count, span.Length - count));
            count += sizeof(ushort);
            name = Encoding.Unicode.GetString(span.Slice(count, nameLen));
            count += nameLen;

            //skill List
            skills.Clear();

            ushort skillLen = (ushort)BitConverter.ToInt16(span.Slice(count, span.Length - count));
            count += sizeof(ushort);

            for(int i=0; i<skillLen;i++)
            {
                Skill skill = new Skill();
                skill.Read(span, ref count);
                skills.Add(skill);
            }
            

        }

        public  ArraySegment<byte> Write()
        {
            ushort count = 0;
            bool success = true;

            ArraySegment<byte> sg = SendBufferHelper.Open(4096);
            Span<byte> span = new Span<byte>(sg.Array, sg.Offset, sg.Count); // span을 만든 이유는 어차피 spawn을 인자로 써야하기때문에

            count += sizeof(ushort);
            success &= BitConverter.TryWriteBytes(span.Slice(count,span.Length-count), (ushort)PacketID.PlayerInfoReq); // 시작범위부터 남은공간까지 오른쪽에 인자 값을 넣어라
            count += sizeof(ushort);
            success &= BitConverter.TryWriteBytes(span.Slice(count,span.Length-count), this.playerId);
            count += sizeof(long);
            

            //string 넣기 unity에서는 string이 기본적으로 utf-16 사용중       
            ushort nameLen = (ushort)Encoding.Unicode.GetBytes(name, 0, name.Length, sg.Array, sg.Offset + count+sizeof(ushort));
            success &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), nameLen);
            count += sizeof(ushort);
            count += nameLen;


            //skill list
            success &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), (ushort)skills.Count);
            count += sizeof(ushort);

            foreach(Skill skill in skills)
            {
                success &= skill.Write(span, ref count);
            }

            success &= BitConverter.TryWriteBytes(span, count);

            if (success == false)
                return null;
            return  SendBufferHelper.Close(count);

        }
    }
   

    public enum PacketID
    {
        PlayerInfoReq =1,
        PlayerInfoOk = 2,
    }

    public class ServerSession : Session
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");

            PlayerInfoReq packet = new PlayerInfoReq() {playerId = 1001, name = "ABCD"};
            packet.skills.Add(new PlayerInfoReq.Skill() { id = 101, level = 50, duration = 3.4f });
            packet.skills.Add(new PlayerInfoReq.Skill() { id = 102, level = 30, duration = 2.4f });
            packet.skills.Add(new PlayerInfoReq.Skill() { id = 103, level = 10, duration = 4.4f });
            packet.skills.Add(new PlayerInfoReq.Skill() { id = 104, level = 5, duration = 1.4f });
            // 보내기 위함
            //for (int i = 0; i < 5; i++)
            {
                ArraySegment<byte> sg = packet.Write();

                if (sg != null)
                    Send(sg);

            }
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
