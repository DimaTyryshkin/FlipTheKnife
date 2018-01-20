using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;

using ScsService.Common;
using Hik.Communication.Scs.Communication.Messengers;
using Hik.Communication.Scs.Communication.Messages;
using Hik.Communication.Scs.Server;

namespace ScsService.Server
{
    /// <summary>
    /// События асинхронно приходящие от клиентов класс складывает в очередь
    /// и позволяет их потом получить из главного игрового цикла
    /// </summary>
    public class ConcurrentEventQueue
    {
        ConcurrentQueue<ReceivedClientEvent> _eventQueue;




        public ConcurrentEventQueue()
        {
            _eventQueue = new ConcurrentQueue<ReceivedClientEvent>();
        }
        
        public void EnqueueEvent(ReceivedClientEvent clientEvent)
        {
            _eventQueue.Enqueue(clientEvent);
        }

        public bool TryDequeue(out ReceivedClientEvent msg)
        {
            return _eventQueue.TryDequeue(out msg);
        }
    }
}