using Hik.Communication.Scs.Communication.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToonKnife.Common.ScsMessages
{
    [Serializable]
    public class AuthenticationMessage : ScsMessage
    {
        public string login;

        public AuthenticationMessage()
        {

        }

        public AuthenticationMessage(string repliedMessageId) : base(repliedMessageId)
        {

        }
    }
}