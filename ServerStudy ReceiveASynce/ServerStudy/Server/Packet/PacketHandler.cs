using Server;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;

class PacketHandler // 해당패킷이 다 조립되면 무엇을 할지 맞춰주는 역할
{
    public static void C_ChatHandler(PacketSession session, IPacket packet)
    { 
        C_Chat chatPacket = packet as C_Chat;
        ClientSession clientSession = session as ClientSession;

        if (clientSession.Room == null)
            return;

        GameRoom room = clientSession.Room;
        room.Push(
                () => room.Broadcast(clientSession, chatPacket.chat)
            );
    }
 
}

