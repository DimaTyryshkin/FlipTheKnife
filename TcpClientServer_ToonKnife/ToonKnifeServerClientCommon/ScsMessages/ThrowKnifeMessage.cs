using System;
using Assets.game.model.knife;
using Hik.Communication.Scs.Communication.Messages;


namespace ToonKnife.Common.ScsMessages
{
    public class ThrowKnifeMessage : ScsMessage
    {
        public float Input { get; }
        public DateTime TimeNextThrow { get; }
        public DateTime TimeThrow { get; }


        public ThrowKnifeMessage(float input, DateTime timeThrow, DateTime timeNextThrow, string repliedMessageId = null) : base(repliedMessageId)
        {
            Input = input;
            TimeNextThrow = timeNextThrow;
            TimeThrow = timeThrow;
        }
    }
}