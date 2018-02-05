﻿using System;
using Assets.game.model.knife;
using Hik.Communication.Scs.Communication.Messages;


namespace ToonKnife.Common.ScsMessages
{
    public class StartFightMessage : ScsMessage
    {
        public StartFightMessage(string repliedMessageId = null) : base(repliedMessageId)
        {
        }
    }
}