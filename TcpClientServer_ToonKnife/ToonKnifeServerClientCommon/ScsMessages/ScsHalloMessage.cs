using Hik.Communication.Scs.Communication.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToonKnife.Common.ScsMessages
{
    [Serializable]
    public class ScsHalloMessage : ScsMessage
    {
        public string HalloText = "Hallo custom msg";

        public ScsHalloMessage(string repliedMessageId) : base(repliedMessageId)
        {
        }
    }
}