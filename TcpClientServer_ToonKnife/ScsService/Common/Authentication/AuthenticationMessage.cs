using Hik.Communication.Scs.Communication.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScsService.Common.Authentication
{
    [Serializable]
    public class AuthenticationMessage : ScsMessage
    {
        public string login;

        public AuthenticationMessage(string login) : this(login, null)
        {

        }

        public AuthenticationMessage(string login, string repliedMessageId) : base(repliedMessageId)
        {
            this.login = login;
        }
    }
}