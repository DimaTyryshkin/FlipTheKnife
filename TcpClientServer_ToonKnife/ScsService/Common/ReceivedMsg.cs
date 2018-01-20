using Hik.Communication.Scs.Communication.Messages;
using Hik.Communication.Scs.Communication.Messengers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;

namespace ScsService.Common
{
    public struct ReceivedMsg
    {
        /// <summary>
        /// Кто прислал
        /// </summary>
        public IMessenger Sender { get; private set; }

        /// <summary>
        /// Полученное сообщение
        /// </summary>
        public IScsMessage Msg { get; private set; }

        public ReceivedMsg(IMessenger sender, IScsMessage msg)
        {
            Sender = sender;
            Msg = msg;
        }
    }
}
