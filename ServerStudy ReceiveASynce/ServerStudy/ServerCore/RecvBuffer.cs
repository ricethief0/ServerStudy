using System;
using System.Collections.Generic;
using System.Text;

namespace ServerCore
{
    class RecvBuffer
    {
        ArraySegment<byte> m_buffer;
        int m_readPos;
        int m_writePos;

        public RecvBuffer(int bufferSize)
        {
            m_buffer = new ArraySegment<byte>(new byte[bufferSize], 0, bufferSize);
        }

        public int DataSize { get { return m_writePos - m_readPos; } }
        public int FreeSize { get { return m_buffer.Count - m_writePos; } }

        public ArraySegment<byte> ReadSegment
        { 
            get { return new ArraySegment<byte>(m_buffer.Array,m_buffer.Offset+m_readPos, DataSize); } 
        }
        public ArraySegment<byte> WriteSegment
        { 
            get { return new ArraySegment<byte>(m_buffer.Array,m_buffer.Offset+m_writePos,FreeSize); } 
        }

        public void Clean()
        {
            if (DataSize.Equals(0)) // 남은 데이터가 없으므로 처음으로 초기화
            {
                m_readPos = m_writePos = 0;
            }
            else
            {
                //남은 데이터가 있으므로 처음 위치로 복사
                Array.Copy(m_buffer.Array, m_buffer.Offset + m_readPos, m_buffer.Array, m_buffer.Offset, DataSize);
                m_readPos = 0;
                m_writePos = DataSize;
            }
        }
        public bool OnRead(int numOfBytes)
        {
            if (numOfBytes > DataSize)
                return false;

            m_readPos += numOfBytes;
            return true;
        }

        public bool OnWrite(int numOfBytes)
        {
            if (numOfBytes > FreeSize)
                return false;

            m_writePos += numOfBytes;
            return true;
        }
    }
}
