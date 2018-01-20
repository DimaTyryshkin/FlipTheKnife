using Hik.Communication.Scs.Communication.Messages;
using Hik.Communication.Scs.Communication.Messengers;
using Hik.Communication.Scs.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;

namespace ScsService.Server
{
    public struct ReceivedClientEvent
    {
        public enum Event
        {
            Connected = 0,
            Disconnected = 1,
        }

        /// <summary>
        /// Кто прислал
        /// </summary>
        public IScsServerClient Client { get; private set; }

        public Event EventType { get; private set; }

        public ReceivedClientEvent(IScsServerClient sender, Event eventType)
        {
            Client = sender;
            EventType = eventType;
        }
    }
}