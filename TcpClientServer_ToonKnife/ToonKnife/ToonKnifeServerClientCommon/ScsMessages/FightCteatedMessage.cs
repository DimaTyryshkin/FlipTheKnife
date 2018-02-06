using System;
using Assets.game.model.knife;
using Hik.Communication.Scs.Communication.Messages;


namespace ToonKnife.Common.ScsMessages
{
    public class FightCteatedMessage : ScsMessage
    {
        public string EnemyUserName { get; }
        public string EnemyKnifeName { get; }
        public KnifeMode EnemyKnifeMode { get; }

        public FightCteatedMessage(string userName, string knifeName, KnifeMode knifeMode, string repliedMessageId = null) : base(repliedMessageId)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentException("message", nameof(userName));
            }

            if (string.IsNullOrEmpty(knifeName))
            {
                throw new ArgumentException("message", nameof(knifeName));
            }

            EnemyUserName = userName;
            EnemyKnifeName = knifeName;
            EnemyKnifeMode = knifeMode;
        }
    }
}