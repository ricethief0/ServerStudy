using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    public class GameRoom
    {
        List<ClientSession> sessions = new List<ClientSession>();
        object m_lock = new object();
        public void Enter(ClientSession session)
        {
            lock(m_lock)
            {
                sessions.Add(session);
                session.Room = this;
            }
            

        }

        public void Broadcast(ClientSession session, string chat)
        {
            S_Chat packet = new S_Chat();
            packet.playerId = session.SeesionId;
            packet.chat = chat;
            ArraySegment<byte> segment = packet.Write();

            lock(m_lock)
            {
                foreach(ClientSession s in sessions)
                {
                    s.Send(segment);
                }    
            }
        }

        public void Leave(ClientSession session)
        {
            lock (m_lock)
            {
                sessions.Remove(session); 
            }
        }
    }
}
