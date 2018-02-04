using Hik.Communication.Scs.Communication.Messages;
using Hik.Communication.Scs.Server;

namespace ScsService.Common
{
    public delegate void ReadMsgDelegate<TScsMessage>(ReceivedMsg receivedMsg, TScsMessage msg);
}