using Hik.Communication.Scs.Communication.Messages;
using System;

namespace ToonKnife.Common.ScsMessages
{
    [Serializable]
    public class GameErrorMessage : ScsMessage
    {
        public enum Error
        {
            UncnownError = -1,
            NoError = 0,
        }

        public Error error;
        public string additinalInfo;

        public GameErrorMessage(Error error, string additinalInfo, string repliedMessageId) : base(repliedMessageId)
        {
            this.error = error;
            this.additinalInfo = additinalInfo;
        }
    }
}