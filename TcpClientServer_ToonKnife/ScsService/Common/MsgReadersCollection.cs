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
        Dictionary<Type, Delegate> _msgReaders;




        public MsgReadersCollection()
        {
            _msgReaders = new Dictionary<Type, Delegate>();
        }

        public void RegisterMsgReader<TMesage>(ReadMsgDelegate<TMesage> reader)
        {
            _msgReaders.Add(typeof(TMesage), reader);
        }

        /// <summary>
        /// Среди зарегистророванных методов находит подходящий по типу сообщения и вызывает его.
        /// </summary> 
        public void CallReader(ReceivedMsg msg)
        {
            var reader = _msgReaders[msg.Msg.GetType()];

            reader.DynamicInvoke(msg, msg.Msg);
        }



        /// <summary>
        /// Для непосредственной подписки у клиента\сервера. Будет рабоатть асинхронно.
        /// </summary>
        /// <param name="sender"><see cref="IMessenger"/></param> 
        public void CallReader(object sender, MessageEventArgs e)
        {
            ReceivedMsg msg = new ReceivedMsg((IMessenger)sender, e.Message);

            //var type= typeof(List<>).MakeGenericType(e.Message.GetType());
            //Activator.cre

            CallReader(msg);
        }
    }
}