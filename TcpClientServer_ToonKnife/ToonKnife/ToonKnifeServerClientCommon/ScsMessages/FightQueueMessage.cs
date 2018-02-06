using System;
using Assets.game.model.knife;
using Hik.Communication.Scs.Communication.Messages;


namespace ToonKnife.Common.ScsMessages
{
    public class FightQueueMessage : ScsMessage
    {
        public string KnifeName { get; }
        public KnifeMode KnifeMode { get; }

        public FightQueueMessage(string knifeName, KnifeMode knifeMode, string repliedMessageId = null) : base(repliedMessageId)
        {
            if (string.IsNullOrEmpty(knifeName))
            {
                throw new ArgumentException("message", nameof(knifeName));
            }

            KnifeName = knifeName;
            KnifeMode = knifeMode;
        }
    }
}