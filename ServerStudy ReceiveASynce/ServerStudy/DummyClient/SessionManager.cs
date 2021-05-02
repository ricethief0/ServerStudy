using System;
using System.Collections.Generic;
using System.Text;

namespace DummyClient
{
    class SessionManager
    {
        static SessionManager m_session = new SessionManager();
        public static SessionManager Instance { get { return m_session; } }

        List<ServerSession> m_sessions = new List<ServerSession>();
        object m_lock = new object();
        public void SendForEach()
        {
            lock (m_lock)
            {
                foreach (ServerSession session in m_sessions)
                {
                    C_Chat chatPacket = new C_Chat();
                    chatPacket.chat = $"Hello Server!";
                    ArraySegment<byte> sg = chatPacket.Write();

                    session.Send(sg);
                }
            }
        }
        public ServerSession Generate()
        {
            lock (m_lock)
            {
                ServerSession session = new ServerSession();
                m_sessions.Add(session);
                return session;
            }
        }
    }
}
