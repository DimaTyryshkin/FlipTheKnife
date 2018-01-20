using Hik.Communication.Scs.Communication.Messages;
using Hik.Communication.Scs.Server;

namespace ToonKnife.Common
{
    public delegate void ReadScsMessageDelegate(IScsServerClient client, IScsMessage msg);
}