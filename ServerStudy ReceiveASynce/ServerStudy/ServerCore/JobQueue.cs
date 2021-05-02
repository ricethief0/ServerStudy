using System;
using System.Collections.Generic;
using System.Text;

namespace ServerCore
{
    public interface IJobQueue
    {
        void Push(Action job);
    }
    public class JobQueue :IJobQueue
    {
        Queue<Action> m_jobQueue = new Queue<Action>();
        object m_lock = new object();
        bool m_flush = false;

        public void Push(Action job)
        {
            bool flush = false;
            /*
             * 별도로 flush를 하나 더 만들어 놓은 이유는 만약 라이브러리이기 때문에 다른 스레드들과 공유중 
             * 즉, 다른 스레드들도 들어온다. 따라서, 각자 스레드 마다에 개인 불리언을(flush) 만들어 
             * m_flush(전체상황)과 공유하여 확인하는것.
             */
            lock (m_lock)
            {
                m_jobQueue.Enqueue(job);
                if(!m_flush)
                    flush = m_flush = true;
                
            }
            if (flush) 
               Flush();
            
        }
        void Flush()
        {
            while(true)
            {
                Action action = Pop();
                if (action == null)
                    return;

                action.Invoke();
            }
        }
        Action Pop()
        {
            lock(m_lock) //여기에도 락을 거는 이유는 push에서 잡큐에 pop하는 순간에 또 넣을 수 있는 상황을 보호하기위해
            {
                if (m_jobQueue.Count <= 0)
                {
                    m_flush = false;
                    return null; 
                }

                return m_jobQueue.Dequeue();
            }
        }
    }
}
