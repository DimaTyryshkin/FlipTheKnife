using System;
using Assets.game.model.knife;
using Hik.Communication.Scs.Communication.Messages;


namespace ToonKnife.Common.ScsMessages
{
    [Serializable]
    public class ThrowKnifeMessage : ScsMessage
    {
        public int KnifeId;

        public float Input { get; }
        public DateTime TimeNextThrow { get; }
        public DateTime TimeThrow { get; }

        public ThrowKnifeMessage(float input)
        {
            Input = input;
        }

        public ThrowKnifeMessage(float input, int knifeId, DateTime timeThrow, DateTime timeNextThrow, string repliedMessageId = null) : base(repliedMessageId)
        {
            Input = input;
            KnifeId = knifeId;
            TimeNextThrow = timeNextThrow;
            TimeThrow = timeThrow;
        }
    }
}