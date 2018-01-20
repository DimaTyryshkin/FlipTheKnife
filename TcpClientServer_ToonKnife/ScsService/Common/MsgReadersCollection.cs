using Hik.Communication.Scs.Communication.Messages;
using Hik.Communication.Scs.Communication.Messengers;
using Hik.Communication.Scs.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScsService.Common
{
    public class MsgReadersCollection
    {
        Dictionary<Type, ReadMsgDelegate> _msgReaders;




        public MsgReadersCollection()
        {
            _msgReaders = new Dictionary<Type, ReadMsgDelegate>();
        }

        public void RegisterMsgReader(Type msgType, ReadMsgDelegate reader)
        {
            _msgReaders.Add(msgType, reader);
        }

        /// <summary>
        /// Среди зарегистророванных методов находит подходящий по типу сообщения и вызывает его.
        /// </summary> 
        public void CallReader(ReceivedMsg msg)
        {
            var reader = _msgReaders[msg.Msg.GetType()];

            reader.Invoke(msg);
        }

        /// <summary>
        /// Для непосредственной подписки у клиента\сервера. Будет рабоатть асинхронно.
        /// </summary>
        /// <param name="sender"><see cref="IMessenger"/></param> 
        public void AsyncClient_MessageReceivedHendler(object sender, MessageEventArgs e)
        {
            ReceivedMsg msg = new ReceivedMsg((IMessenger)sender, e.Message);
            CallReader(msg);
        }
    }
}