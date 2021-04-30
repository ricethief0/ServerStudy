
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using ServerCore;

public enum PacketID
{
    PlayerInfoReq = 1,
	Test = 2,
	
}


public class PlayerInfoReq  //body
{
    public byte testByte;
	public long playerId;
	public string name;
	
	public class Skill
	{
	    public int id;
		public short level;
		public float duration;
	
	    public void Read(ReadOnlySpan<byte> span, ref ushort count)
	    {
	        this.id = BitConverter.ToInt32(span.Slice(count,span.Length-count));
			    count += sizeof(int); 
			this.level = BitConverter.ToInt16(span.Slice(count,span.Length-count));
			    count += sizeof(short); 
			this.duration = BitConverter.ToSingle(span.Slice(count,span.Length-count));
			    count += sizeof(float); 
	    }
	    public bool Write(Span<byte> span, ref ushort count)
	    {
	        bool success = true;
	        success &= BitConverter.TryWriteBytes(span.Slice(count,span.Length-count), this.id);
			count += sizeof(int);
			success &= BitConverter.TryWriteBytes(span.Slice(count,span.Length-count), this.level);
			count += sizeof(short);
			success &= BitConverter.TryWriteBytes(span.Slice(count,span.Length-count), this.duration);
			count += sizeof(float);
	        return success;
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

        this.testByte = (byte)sg.Array[sg.Offset + count];
		count += sizeof(byte);
		this.playerId = BitConverter.ToInt64(span.Slice(count,span.Length-count));
		    count += sizeof(long); 
		ushort nameLen = BitConverter.ToUInt16(span.Slice(count, span.Length - count));
		count += sizeof(ushort);
		name = Encoding.Unicode.GetString(span.Slice(count, nameLen));
		count += nameLen;
		
		skills.Clear();
		ushort skillLen = (ushort)BitConverter.ToInt16(span.Slice(count, span.Length - count));
		count += sizeof(ushort);
		for (int i = 0; i < skillLen; i++)
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
        sg.Array[sg.Offset + count] = (byte)this.testByte;
		count += sizeof(byte);
		success &= BitConverter.TryWriteBytes(span.Slice(count,span.Length-count), this.playerId);
		count += sizeof(long);
		ushort nameLen = (ushort)Encoding.Unicode.GetBytes(this.name, 0, this.name.Length, sg.Array, sg.Offset + count+sizeof(ushort));
		success &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), nameLen);
		count += sizeof(ushort);
		count += nameLen;
		success &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), (ushort)this.skills.Count);
		count += sizeof(ushort);
		foreach (Skill skill in this.skills)
		    success &= skill.Write(span, ref count);
        success &= BitConverter.TryWriteBytes(span, count);
        if (success == false)
            return null;
        return  SendBufferHelper.Close(count);

    }
}

public class Test  //body
{
    public int testInt;

    public  void Read(ArraySegment<byte> sg)
    {

        ushort count = 0;

        ReadOnlySpan<byte> span = new Span<byte>(sg.Array,sg.Offset,sg.Count);

        //ushort size = BitConverter.ToUInt16(sg.Array, sg.Offset);
        count += sizeof(ushort);
        //ushort id = BitConverter.ToUInt16(sg.Array, sg.Offset + count);
        count += sizeof(ushort);

        this.testInt = BitConverter.ToInt32(span.Slice(count,span.Length-count));
		    count += sizeof(int); 
            

    }

    public  ArraySegment<byte> Write()
    {
        ushort count = 0;
        bool success = true;

        ArraySegment<byte> sg = SendBufferHelper.Open(4096);
        Span<byte> span = new Span<byte>(sg.Array, sg.Offset, sg.Count); // span을 만든 이유는 어차피 spawn을 인자로 써야하기때문에

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(span.Slice(count,span.Length-count), (ushort)PacketID.Test); // 시작범위부터 남은공간까지 오른쪽에 인자 값을 넣어라
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(span.Slice(count,span.Length-count), this.testInt);
		count += sizeof(int);
        success &= BitConverter.TryWriteBytes(span, count);
        if (success == false)
            return null;
        return  SendBufferHelper.Close(count);

    }
}

