using System;
using System.Collections.Generic;
using System.Linq;
using Assets.game.model.knife;
using ScsService.Common;
using ScsService.Server;
using ToonKnife.Common.ScsMessages;
using ToonKnife.Server.Fight;

namespace ToonKnife.Server.ScsServiceAdapter.Controllers
{
    public class UserFighterController : IFighterController
    {
        User _user;
        Fight.Fight _fight;
        int _knifeIndex;
        bool _waitForReady;

        public string Login => _user.Login;

        public UserFighterController(User user, Fight.Fight fight, int knifeIndex)
        {
            _user = user ?? throw new ArgumentNullException(nameof(user));
            _fight = fight ?? throw new ArgumentNullException(nameof(fight));
            _knifeIndex = knifeIndex;

            _fight.Win += Fight_Win_Handler;
            _fight.FightStart += Fight_FightStart_Handler;
            _fight.DeatHead += Fight_DeatHead_Handler;
            _fight.KnifeThrow += Kight_KnifeThrow_Handler;

            _user.MsgReaders.RegisterMsgReader<ThrowKnifeMessage>(UserThroKnife_Reader);
            _user.MsgReaders.RegisterMsgReader<ReadyToFightMessage>(UserReadyToFight_Reader);
        }


        public void SendFightCreated(string enemyName, string enemyKnifeName, KnifeMode enemyKnifeMode)
        {
            _waitForReady = true;

            var msg = new FightCteatedMessage(_knifeIndex, enemyName, enemyKnifeName, enemyKnifeMode);
            _user.SendMsg(msg);
        }

        //---handlers
        private void Fight_DeatHead_Handler(Fight.Fight obj)
        {
            _user.SendMsg(new EndFightMessage(-1, isDeatHead: true));
        }

        void Fight_FightStart_Handler(Fight.Fight obj)
        {
            var startFightMessage = new StartFightMessage();
            _user.SendMsg(startFightMessage);
        }

        void Kight_KnifeThrow_Handler(object sender, KnifeThrowEventArgs e)
        {
            _user.SendMsg(new ThrowKnifeMessage(e.input, e.knifeIndex, e.timeThrow, e.timeNextThrow));
        }

        void Fight_Win_Handler(object sender, int winerIndex)
        {
            _user.SendMsg(new EndFightMessage(_knifeIndex, isDeatHead: false));
        }

        //---readers
        void UserThroKnife_Reader(ReceivedMsg receivedMsg, ThrowKnifeMessage msg)
        {
            if (_fight.CanThrow(_knifeIndex))
            {
                _fight.ThrowKnife(_knifeIndex, msg.Input);
            }
        }

        void UserReadyToFight_Reader(ReceivedMsg receivedMsg, ReadyToFightMessage msg)
        {
            if (_waitForReady)
            {
                _waitForReady = false;

                if (_fight.WaitForReady)
                {
                    _fight.SetOneKnifeReady(_knifeIndex);
                }
            }
        }
    }
}