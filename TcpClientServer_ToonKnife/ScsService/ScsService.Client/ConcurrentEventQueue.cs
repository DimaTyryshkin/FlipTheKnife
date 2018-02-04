using System;
using System.Collections.Generic; 
using System.Linq;

using ScsService.Common;
using Hik.Communication.Scs.Communication.Messengers;
using Hik.Communication.Scs.Communication.Messages;
using Hik.Communication.Scs.Server;

namespace ScsService.Client
{
    /// <summary>
    /// События асинхронно приходящие от клиентов класс складывает в очередь
    /// и позволяет их потом получить из главного игрового цикла
    /// </summary>
    internal class ConcurrentEventQueue
    {
        Queue<ClientEvent> _eventQueue;
        object _lockObejct;

        public int Count
        {
            get
            {
                return _eventQueue.Count;
            }
        }


        public ConcurrentEventQueue()
        {
            _lockObejct = new object();
            _eventQueue = new Queue<ClientEvent>();
        }

        public void EnqueueEvent(ClientEvent clientEvent)
        {
            lock (_lockObejct)
            {
                _eventQueue.Enqueue(clientEvent);
            }
        }

        public ClientEvent Dequeue()
        {
            lock (_lockObejct)
            {
                return _eventQueue.Dequeue();
            }
        }
    }
}