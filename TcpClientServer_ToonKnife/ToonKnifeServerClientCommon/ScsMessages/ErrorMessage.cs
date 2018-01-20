using Hik.Communication.Scs.Communication.Messages;
using System; 

namespace ToonKnife.Common.ScsMessages
{
    [Serializable]
    public class ErrorMessage : ScsMessage
    {
        public enum Error
        {
            UncnownError = -1,
            NoError = 0,

            ToManyClientOnServer = 1,
        }

        public Error error;
        public string additinalInfo;

        public ErrorMessage(Error error, string additinalInfo, string repliedMessageId) : base(repliedMessageId)
        {
            this.error = error;
            this.additinalInfo = additinalInfo;
        }
    }
}