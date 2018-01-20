using Hik.Communication.Scs.Communication.Messages;
using Hik.Communication.Scs.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScsServiceInfrastructure
{
    public class MsgReadersCollection
    {
        Dictionary<Type, ReadScsMessageDelegate> msgReaders;

        public MsgReadersCollection()
        {
            msgReaders = new Dictionary<Type, ReadScsMessageDelegate>();
        }

        public void RegisterMsgReader(Type msgType, ReadScsMessageDelegate reader)
        {
            msgReaders.Add(msgType, reader);
        }

        public void MessageReceived_Handler(object sender, MessageEventArgs e)
        {
            var message = e.Message as IScsMessage;
            var client = (IScsServerClient)sender;

            var reader = msgReaders[message.GetType()];

            //reader.Invoke(client, message); 
            reader.DynamicInvoke(client, message);
        }
    }
}