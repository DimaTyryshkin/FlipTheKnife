using System;
using Assets.game.model.knife;
using Hik.Communication.Scs.Communication.Messages;


namespace ToonKnife.Common.ScsMessages
{
    public class EndFightMessage : ScsMessage
    {
        /// <summary>
        /// если -1 значит ничья
        /// </summary>
        public int WinnerKnifeIndex { get; }


        public EndFightMessage(int winnerKnifeIndex, string repliedMessageId = null) : base(repliedMessageId)
        {
            WinnerKnifeIndex = winnerKnifeIndex;
        }
    }
}