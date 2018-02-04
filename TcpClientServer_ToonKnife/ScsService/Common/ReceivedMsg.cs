using Hik.Communication.Scs.Communication.Messages;
using Hik.Communication.Scs.Communication.Messengers; 

namespace ScsService.Common
{
    public class ReceivedMsg
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
