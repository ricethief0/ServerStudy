
using ServerCore;
using System;
using System.Collections.Generic;

class PacketManager
{
    #region singleton
    static PacketManager m_instance = new PacketManager();
    public static PacketManager Instance { get { return m_instance; } }

    #endregion

    PacketManager() { Register(); }

    Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>> m_onRecv = new Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>>();
    Dictionary<ushort, Action<PacketSession, IPacket>> m_handler = new Dictionary<ushort, Action<PacketSession, IPacket>>();

    public void Register()
    {

        m_onRecv.Add((ushort)PacketID.C_Chat, MakePacket<C_Chat>);
        m_handler.Add((ushort)PacketID.C_Chat, PacketHandler.C_ChatHandler);

        
    }
    public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffers)
    {
        ushort count = 0;
        ushort size = BitConverter.ToUInt16(buffers.Array, buffers.Offset);
        count += 2;
        ushort id = BitConverter.ToUInt16(buffers.Array, buffers.Offset + count);
        count += 2;

        Action<PacketSession, ArraySegment<byte>> action = null;
        if (m_onRecv.TryGetValue(id, out action))
            action.Invoke(session, buffers);  
           
    }

    void MakePacket<T>(PacketSession session, ArraySegment<byte> buffers ) where T : IPacket, new()
    {
        T packet = new T();
        packet.Read(buffers);

        Action<PacketSession, IPacket> action = null;
        if(m_handler.TryGetValue(packet.Protocol, out action))
        {
            action.Invoke(session, packet);
        }
    }
}