
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using ServerCore;

public enum PacketID
{
    C_Chat = 1,
	S_Chat = 2,
	
}

interface IPacket
{
	ushort Protocol { get; }
	void Read(ArraySegment<byte> se);
	ArraySegment<byte> Write();
}


class C_Chat :IPacket
{
    public string chat;

    public ushort Protocol { get { return (ushort)PacketID.C_Chat; } }
    public  void Read(ArraySegment<byte> sg)
    {

        ushort count = 0;

        ReadOnlySpan<byte> span = new Span<byte>(sg.Array,sg.Offset,sg.Count);

        //ushort size = BitConverter.ToUInt16(sg.Array, sg.Offset);
        count += sizeof(ushort);
        //ushort id = BitConverter.ToUInt16(sg.Array, sg.Offset + count);
        count += sizeof(ushort);

        ushort chatLen = BitConverter.ToUInt16(span.Slice(count, span.Length - count));
		count += sizeof(ushort);
		chat = Encoding.Unicode.GetString(span.Slice(count, chatLen));
		count += chatLen;
		
            

    }

    public  ArraySegment<byte> Write()
    {
        ushort count = 0;
        bool success = true;

        ArraySegment<byte> sg = SendBufferHelper.Open(4096);
        Span<byte> span = new Span<byte>(sg.Array, sg.Offset, sg.Count); // span을 만든 이유는 어차피 spawn을 인자로 써야하기때문에

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(span.Slice(count,span.Length-count), (ushort)PacketID.C_Chat); // 시작범위부터 남은공간까지 오른쪽에 인자 값을 넣어라
        count += sizeof(ushort);
        ushort chatLen = (ushort)Encoding.Unicode.GetBytes(this.chat, 0, this.chat.Length, sg.Array, sg.Offset + count+sizeof(ushort));
		success &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), chatLen);
		count += sizeof(ushort);
		count += chatLen;
        success &= BitConverter.TryWriteBytes(span, count);
        if (success == false)
            return null;
        return  SendBufferHelper.Close(count);

    }
}

class S_Chat :IPacket
{
    public int playerId;
	public string chat;

    public ushort Protocol { get { return (ushort)PacketID.S_Chat; } }
    public  void Read(ArraySegment<byte> sg)
    {

        ushort count = 0;

        ReadOnlySpan<byte> span = new Span<byte>(sg.Array,sg.Offset,sg.Count);

        //ushort size = BitConverter.ToUInt16(sg.Array, sg.Offset);
        count += sizeof(ushort);
        //ushort id = BitConverter.ToUInt16(sg.Array, sg.Offset + count);
        count += sizeof(ushort);

        this.playerId = BitConverter.ToInt32(span.Slice(count,span.Length-count));
		    count += sizeof(int); 
		ushort chatLen = BitConverter.ToUInt16(span.Slice(count, span.Length - count));
		count += sizeof(ushort);
		chat = Encoding.Unicode.GetString(span.Slice(count, chatLen));
		count += chatLen;
		
            

    }

    public  ArraySegment<byte> Write()
    {
        ushort count = 0;
        bool success = true;

        ArraySegment<byte> sg = SendBufferHelper.Open(4096);
        Span<byte> span = new Span<byte>(sg.Array, sg.Offset, sg.Count); // span을 만든 이유는 어차피 spawn을 인자로 써야하기때문에

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(span.Slice(count,span.Length-count), (ushort)PacketID.S_Chat); // 시작범위부터 남은공간까지 오른쪽에 인자 값을 넣어라
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(span.Slice(count,span.Length-count), this.playerId);
		count += sizeof(int);
		ushort chatLen = (ushort)Encoding.Unicode.GetBytes(this.chat, 0, this.chat.Length, sg.Array, sg.Offset + count+sizeof(ushort));
		success &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), chatLen);
		count += sizeof(ushort);
		count += chatLen;
        success &= BitConverter.TryWriteBytes(span, count);
        if (success == false)
            return null;
        return  SendBufferHelper.Close(count);

    }
}

