using System;
using Assets.game.model.knife;
using Hik.Communication.Scs.Communication.Messages;


namespace ToonKnife.Common.ScsMessages
{
    public class EndFightMessage : ScsMessage
    {
        public bool isWinner;

        /// <summary>
        /// ничья
        /// </summary>
        public bool isDeatHead;

        public EndFightMessage(bool isWinner, bool isDeatHead, string repliedMessageId = null) : base(repliedMessageId)
        {
            this.isWinner = isWinner;
            this.isDeatHead = isDeatHead;
        } 
    }
}