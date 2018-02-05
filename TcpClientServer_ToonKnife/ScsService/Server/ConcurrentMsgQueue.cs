using System; 
using System.Collections.Concurrent; 

using ScsService.Common;
using Hik.Communication.Scs.Communication.Messengers;
using Hik.Communication.Scs.Communication.Messages;
using System.Collections.Generic;
using Hik.Communication.Scs.Server;

namespace ScsService.Server
{
    /// <summary>
    /// Сообщения асинхронно приходящие от мессенджеров(клиенты или серверы) класс складывает в очередь
    /// и позволяет их потом получить из главного игрового цикла
    /// </summary>
    internal class ConcurrentMsgQueue : IConcurrentMsgQueue
    {
        struct Entry
        {
            public ReceivedMsg msg;
            public ClientEvent clientEvent;
        }

        ConcurrentQueue<Entry> _entryQueue;
        MsgReadersCollection _msgReaders;
        Dictionary<IScsServerClient, User> _authenticatedUsers;

        //---events
        public event EventHandler<ClientEvent> ClientEventReaded;

        //---prop

        public int Count => _entryQueue.Count;



        //---methods
        public ConcurrentMsgQueue(MsgReadersCollection msgReaders, Dictionary<IScsServerClient, User> authenticatedUsers)
        {
            _authenticatedUsers = authenticatedUsers;
            _msgReaders = msgReaders;
            _entryQueue = new ConcurrentQueue<Entry>();
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

        public void EnqueueEvent(ClientEvent clientEvent)
        {
            Entry entry = new Entry()
            {
                clientEvent = clientEvent
            };

            _entryQueue.Enqueue(entry);
            Console.WriteLine(GetType().Name + " :Enqueue Event " + clientEvent.EventType);
        }

        public void ReadStoredMsg(int maxCount = 50)
        {
            Entry entry;
            int count = Math.Min(maxCount, _entryQueue.Count);
            int countOriginal = count;

            int n = 1;
            while (count > 0 && _entryQueue.TryDequeue(out entry))
            {
                count--;

                Console.WriteLine(GetType().Name + " :entry " + n + "/" + countOriginal + " curCount=" + _entryQueue.Count);
                if (entry.msg != null)
                {
                    Console.WriteLine(GetType().Name + " :Read Msg " + entry.msg.Msg.GetType().Name);
                    User user;
                    if (_authenticatedUsers.TryGetValue((IScsServerClient)entry.msg.Sender, out user))
                    { 
                        if (user.MsgReaders.CallReader(entry.msg))
                        {
                            continue;
                        }
                    }
                     
                    //TODO проверять что есть метод
                    _msgReaders.CallReader(entry.msg);
                }
                else
                {
                    Console.WriteLine(GetType().Name + " :Read Event");
                    ClientEventReaded.Invoke(this, entry.clientEvent);
                }

                n++;
            }
        }

        void Messenger_MessageReceived(object sender, MessageEventArgs e)
        {
            ReceivedMsg msg = new ReceivedMsg((IMessenger)sender, e.Message);
             
            Entry entry = new Entry()
            {
                msg = msg,
            };

            _entryQueue.Enqueue(entry);
            Console.WriteLine(GetType().Name + " :Enqueue Mgg " + e.Message.GetType().Name);
        }
    }
}