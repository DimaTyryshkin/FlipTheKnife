using Hik.Communication.Scs.Communication.Messages;
using System;

namespace ScsService.Common.Authentication
{
    [Serializable]
    public class AuthenticationSuccesMessage : ScsMessage
    {
        public AuthenticationSuccesMessage(string repliedMessageId=null) : base(repliedMessageId)
        {
        }
    }
}