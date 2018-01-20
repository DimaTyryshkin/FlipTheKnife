using Hik.Communication.Scs.Communication.Messages;
using Hik.Communication.Scs.Server;

namespace ScsServiceInfrastructure
{
    public delegate void ReadScsMessageDelegate(IScsServerClient scsClient, IScsMessage scsMsg);
}