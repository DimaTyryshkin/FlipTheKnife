using Hik.Communication.Scs.Communication.Messages;
using Hik.Communication.Scs.Communication.Messengers; 
using System;
using System.Collections.Generic; 

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

        public void RemoveMsgReader<TMesage>()
        {
            _msgReaders.Remove(typeof(TMesage));
        }

        /// <summary>
        /// Среди зарегистророванных методов находит подходящий по типу сообщения и вызывает его.
        /// </summary> 
        public bool CallReader(ReceivedMsg msg)
        {
            Delegate reader;

            if (_msgReaders.TryGetValue(msg.Msg.GetType(), out reader))
            {
                reader.DynamicInvoke(msg, msg.Msg);

                return true;
            }
            else
            {
                return false;
            }
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