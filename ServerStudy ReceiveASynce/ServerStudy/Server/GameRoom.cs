using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    public class GameRoom :IJobQueue
    {
        List<ClientSession> sessions = new List<ClientSession>();
        JobQueue m_jobQueue = new JobQueue();
        List<ArraySegment<byte>> m_pendingList = new List<ArraySegment<byte>>();

        public void Push(Action job)
        {
            m_jobQueue.Push(job);
        }
        public void Enter(ClientSession session)
        {
            sessions.Add(session);
            session.Room = this;
        }

        public void Broadcast(ClientSession session, string chat)
        {
            S_Chat packet = new S_Chat();
            packet.playerId = session.SeesionId;
            packet.chat = $"{chat} I am {packet.playerId}";
            ArraySegment<byte> segment = packet.Write();

            m_pendingList.Add(segment);
        }

        public void Flush()
        {
            foreach (ClientSession s in sessions)
                s.Send(m_pendingList);

            Console.WriteLine($"Flushed {m_pendingList.Count} Items");
            m_pendingList.Clear();
        }

        public void Leave(ClientSession session)
        {
            sessions.Remove(session); 
        }
    }
}
