using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    class SessionManager
    {
        static SessionManager m_session = new SessionManager();
        public static SessionManager Instance { get { return m_session; } }

        int m_sessionId = 0;
        Dictionary<int, ClientSession> m_sessins = new Dictionary<int, ClientSession>();
        object m_lock = new object();

        public ClientSession Create()
        {
            lock(m_lock)
            {
                int sessionId = ++m_sessionId;

                ClientSession session = new ClientSession(); // 풀링해서 해도 됌.
                session.SeesionId = sessionId;
                m_sessins.Add(sessionId, session);
                
                Console.WriteLine($"ConnectId : {sessionId}");

                return session;

            }
        }

        public ClientSession Find(int id)
        {
            lock (m_lock)
            {
                ClientSession session = null;
                m_sessins.TryGetValue(id, out session);
                return session;
            }
        }

        public void Remove(ClientSession session)
        {
            lock(m_lock)
            {
                m_sessins.Remove(session.SeesionId);
            }
        }
    }
}
