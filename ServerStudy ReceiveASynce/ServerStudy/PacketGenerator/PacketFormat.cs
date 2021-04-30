using System;
namespace PacketGenerator
{
    public class PacketFormat
    {

        //{0} 패킷 이름/번호 목록
        //{1} 패킷 목록
        public static string fileFormat =
@"
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using ServerCore;

public enum PacketID
{{
    {0}
}}

{1}
";
        // {0} 패킷 이름
        // {1} 패킷 번호
        public static string packetEnumFormat =
@"{0} = {1},";
        // {0} 패킷 이름
        // {1} 멤버 변수들
        // {2} 멤버 변수 Read
        // {3} 멤버 변수 Write
        public static string packetFormat =
@"
class {0}  //body
{{
    {1}

    public  void Read(ArraySegment<byte> sg)
    {{

        ushort count = 0;

        ReadOnlySpan<byte> span = new Span<byte>(sg.Array,sg.Offset,sg.Count);

        //ushort size = BitConverter.ToUInt16(sg.Array, sg.Offset);
        count += sizeof(ushort);
        //ushort id = BitConverter.ToUInt16(sg.Array, sg.Offset + count);
        count += sizeof(ushort);

        {2}
            

    }}

    public  ArraySegment<byte> Write()
    {{
        ushort count = 0;
        bool success = true;

        ArraySegment<byte> sg = SendBufferHelper.Open(4096);
        Span<byte> span = new Span<byte>(sg.Array, sg.Offset, sg.Count); // span을 만든 이유는 어차피 spawn을 인자로 써야하기때문에

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(span.Slice(count,span.Length-count), (ushort)PacketID.{0}); // 시작범위부터 남은공간까지 오른쪽에 인자 값을 넣어라
        count += sizeof(ushort);
        {3}
        success &= BitConverter.TryWriteBytes(span, count);
        if (success == false)
            return null;
        return  SendBufferHelper.Close(count);

    }}
}}
";
        //{0} 변수 형식
        //{1} 변수 이름
        public static string memberFormat =
@"public {0} {1};";

        // {0} 리스트 이름 [대문자]
        // {1} 리스트 이름 [소문자]
        // {2} 멤버 변수들
        // {3} 멤버 변수 Read
        // {4} 멤버 변수 Write

        public static string memberListFormat =
@"
 public class {0}
        {{
            {2}

            public void Read(ReadOnlySpan<byte> span, ref ushort count)
            {{
                {3}
            }}
            public bool Write(Span<byte> span, ref ushort count)
            {{
                bool success = true;
                {4}
                return success;
            }}
        }}

        public List<{0}> {1]s = new List<{0}>();

";

        //{0} 변수 이름
        //{1} To~ 변수 형식
        //{2} 변수 형식
        public static string readFormat =
@"this.{0} = BitConverter.{1}(span.Slice(count,span.Length-count));
    count += sizeof({2}); ";

        // {0} 변수 이름
        // {1} 변수 형식
        public static string readByteFormat =
@"this.{0} = ({1})segment.Array[sg.Offset + count];
count += sizeof({1})";

        //{0] 변수이름
        public static string readStringFormat =
@"ushort {0}Len = BitConverter.ToUInt16(span.Slice(count, span.Length - count));
count += sizeof(ushort);
{0} = Encoding.Unicode.GetString(span.Slice(count, {0}Len));
count += {0}Len;
";

        //{0} 리스트 이름 [대문자]
        //{1} 리스트 이름 [소문자]

        public static string readListFormat =
@"{1}s.Clear();
ushort {1}Len = (ushort)BitConverter.ToInt16(span.Slice(count, span.Length - count));
count += sizeof(ushort);
for (int i = 0; i < {1}Len; i++)
{{
    {0} {1} = new {0}();
    {1}.Read(span, ref count);
    {1}s.Add({1});
}}";
        //{0} 변수이름
        //{1} 변수형식
        public static string writeFormat =
@"success &= BitConverter.TryWriteBytes(span.Slice(count,span.Length-count), this.{0});
count += sizeof({1});";


        // {0} 변수 이름
        // {1} 변수 형식
        public static string writeByteFormat =
@"segment.Array[sg.Offset + count] = ({1})this.{0};
count += sizeof({1})";

        //{0} 변수이름
        public static string writeStringFormat =
@"ushort {0}Len = (ushort)Encoding.Unicode.GetBytes(this.{0}, 0, this.{0}.Length, sg.Array, sg.Offset + count+sizeof(ushort));
success &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), {0}Len);
count += sizeof(ushort);
count += {0}Len;";

        //{0} 리스트 이름 [대문자]
        //{1} 리스트 이름 [소문자]

        public static string writeListFormat =
@"success &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), (ushort)this.{1}s.Count);
count += sizeof(ushort);
foreach ({0} {1} in this.{1}s)
    success &= {1}.Write(span, ref count);";

    }
}
