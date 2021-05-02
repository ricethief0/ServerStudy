using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ServerCore
{
    public class SendBufferHelper
    {
        public static ThreadLocal<SendBuffer> CurrentBuffer = new ThreadLocal<SendBuffer>(()=> { return null; });

        public static int ChunkSize { get; set; } = 65535*100;
        public static ArraySegment<byte> Open(int reserveSize)
        {
            if (CurrentBuffer.Value == null)
                CurrentBuffer.Value = new SendBuffer(ChunkSize);

            if (CurrentBuffer.Value.FreeSize < reserveSize)
                CurrentBuffer.Value = new SendBuffer(ChunkSize);

            return CurrentBuffer.Value.Open(reserveSize);
            
        }
        public static ArraySegment<byte> Close(int usedSize)
        {
            return CurrentBuffer.Value.Close(usedSize);
        }
    }

    public class SendBuffer
    {
        byte[] m_buffer;
        int m_usedSize = 0; // recev에서 writebuffer와 유사

        public int FreeSize { get { return m_buffer.Length - m_usedSize; } }

        public SendBuffer(int chunkSize)
        {
            m_buffer = new byte[chunkSize];
        }
        public ArraySegment<byte> Open(int reserveSize)
        {
            if (reserveSize > FreeSize)
                return null;

            return new ArraySegment<byte>(m_buffer, m_usedSize, reserveSize);
        }
        public ArraySegment<byte> Close(int usedSize)
        {
            ArraySegment<byte> segment = new ArraySegment<byte>(m_buffer, m_usedSize, usedSize);
            m_usedSize += usedSize;
            return segment;
        }
    }
}
