using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;

using ScsService.Common;
using Hik.Communication.Scs.Communication.Messengers;
using Hik.Communication.Scs.Communication.Messages;

namespace ScsService.Server
{
    /// <summary>
    /// Сообщения асинхронно приходящие от мессенджеров(клиенты или серверы) класс складывает в очередь
    /// и позволяет их потом получить из главного игрового цикла
    /// </summary>
    public class ConcurrentMsgQueue : IConcurrentMsgQueue
    {
        ConcurrentQueue<ReceivedMsg> _msgQueue;


        
        public ConcurrentMsgQueue()
        {
            _msgQueue = new ConcurrentQueue<ReceivedMsg>();
        }

        /// <summary>
        /// Начать прием сообщений от клиента или сервера
        /// </summary>
        /// <param name="messenger">клиент или сервер</param>
        public void AddMessenger(IMessenger messenger)
        {
            messenger.MessageReceived += Messenger_MessageReceived;
        }

        public ReceivedMsg[] ToArray()
        {
            return _msgQueue.ToArray();
        }

        public void RemoveMessenger(IMessenger messenger)
        {
            messenger.MessageReceived -= Messenger_MessageReceived;
        }

        private void Messenger_MessageReceived(object sender, MessageEventArgs e)
        {
            ReceivedMsg msg = new ReceivedMsg((IMessenger)sender, e.Message);
            _msgQueue.Enqueue(msg);
        }
    }
}