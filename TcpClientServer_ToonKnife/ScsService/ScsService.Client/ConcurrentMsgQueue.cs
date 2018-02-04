using System.Collections.Generic;
using ScsService.Common;
using Hik.Communication.Scs.Communication.Messengers;
using Hik.Communication.Scs.Communication.Messages;

namespace ScsService.Client
{
    /// <summary>
    /// Сообщения асинхронно приходящие от мессенджеров(клиенты или серверы) класс складывает в очередь
    /// и позволяет их потом получить из главного игрового цикла
    /// </summary>
    internal class ConcurrentMsgQueue  
    {
        Queue<ReceivedMsg> _msgQueue;
        MsgReadersCollection _msgReaders;
        object _lockObject;

        public ConcurrentMsgQueue(MsgReadersCollection msgReaders)
        {
            _msgReaders = msgReaders;
            _lockObject = new object();
            _msgQueue = new Queue<ReceivedMsg>();
        }

        /// <summary>
        /// Начать прием сообщений от клиента или сервера
        /// </summary>
        /// <param name="messenger">клиент или сервер</param>
        public void AddMessenger(IMessenger messenger)
        {
            messenger.MessageReceived += Messenger_MessageReceived;
        }

        public void RemoveMessenger(IMessenger messenger)
        {
            messenger.MessageReceived -= Messenger_MessageReceived;
        }

        public void ReadStoredMsg()
        {
            foreach (var msg in ToArrayAndClear())
            {
                _msgReaders.CallReader(msg);
            }
        }

        List<ReceivedMsg> ToArrayAndClear()
        {
            lock (_lockObject)
            {
                var q = _msgQueue.ToArray();
                _msgQueue.Clear();

                return new List<ReceivedMsg>(q);
            }
        }

        void Messenger_MessageReceived(object sender, MessageEventArgs e)
        {
            ReceivedMsg msg = new ReceivedMsg((IMessenger)sender, e.Message);
            lock (_lockObject)
            {
                _msgQueue.Enqueue(msg);
            }
        }
    }
}