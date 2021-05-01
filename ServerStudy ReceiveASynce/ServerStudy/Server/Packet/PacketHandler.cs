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
        clientSession.Room.Broadcast(clientSession, chatPacket.chat);
        //Console.WriteLine($"PlayerId:{req.playerId} / name : {req.name}");

        //foreach (C_Chat.Skill skill in req.skills)
        //{
        //    Console.WriteLine($"skill id : {skill.id} / level : {skill.level} / duration : {skill.duration}");
        //}
    }
 
}

