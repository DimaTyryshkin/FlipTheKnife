using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.game.model.knife;
using IntegrationTestInfrastructure;
using ToonKnife.Server;
using ToonKnife.Server.Fight;

namespace ToonKnife.TestServer
{
    class TestBotFighterController : IFighterController
    {
        Fight _fight;
        int _knifeIndex;
        ToonKnifeTest _test;
        bool _isWolf;
        bool _waitForReady;
        bool _fightIsClosed;

        float[] _input;
        int curInputIndex = 0;

        public string Login { get; private set; }



        public TestBotFighterController(string login, Fight fight, int knifeIndex, ToonKnifeTest test, bool isWolf)
        {
            Login = login ?? throw new ArgumentNullException(nameof(login));
            _fight = fight ?? throw new ArgumentNullException(nameof(fight));
            _knifeIndex = knifeIndex;
            _test = test;
            _isWolf = isWolf;
            _fight.Win += Fight_Win_Handler;
            _fight.FightStart += Fight_FightStart_Handler;
            _fight.FightClose += Fight_FightClose_Handler;

            _fight.KnifeThrow += Fight_KnifeThrow; 

            if (_isWolf)
            { 
                _input = new float[] {
                    1173.6f,
                    1250.4f,
                    1166.4f,
                    1197.6f,
                    1185.6f,
                    1197.6f,
                    1300.8f,
                    1231.2f,
                    1224f,
                    1228.8f,
                    1166.4f,
                    1236f,
                    1216.8f,
                    1231.2f,
                    1228.8f,
                    1152f,
                    1224f,
                    1185.6f,
                    1224f
                };
            }
            else
            {
                _input = new float[] {
                    1068f,
                    };
            }
        }


        public void SendFightCreated(string enemyName, string enemyKnifeName, KnifeMode enemyKnifeMode)
        {
            _waitForReady = true;
            _waitForReady = false;

            _fight.SetOneKnifeReady(_knifeIndex);
        }

        //---handlers
        void Fight_KnifeThrow(object sender, KnifeThrowEventArgs e)
        {
            if (e.knifeIndex == _knifeIndex)
            {
                Timer.AddTimer(Thorow, e.timeNextThrow + TimeSpan.FromSeconds(0.1));
            }
        }

        void Fight_FightClose_Handler(Fight obj)
        {
            _fightIsClosed = true;
        }

        void Fight_FightStart_Handler(Fight obj)
        {
            Thorow();
        }

        void Thorow()
        {
            if (!_fightIsClosed)
            {
                float input = _input[curInputIndex];
                curInputIndex = (curInputIndex + 1) % _input.Length;

                if (_fight.CanThrow(_knifeIndex))
                {
                    Console.WriteLine(typeof(TestBotFighterController).Name + $" Login={Login} throw knife");
                    _fight.ThrowKnife(_knifeIndex, input);
                }
                else
                {
                    throw new Exception("Бот не работает, не может толкнуть нож");
                }
            }
        }


        void Fight_Win_Handler(object sender, int winerIndex)
        {
            if (_isWolf)
            {
                if (winerIndex == _knifeIndex)
                {
                    _test.Step_AllWolfUsers_SendWin.IncrimentCounter(Login);
                }
            }
            else
            {
                if (winerIndex != _knifeIndex)
                {
                    _test.Step_AllRabbitUsers_SendFail.IncrimentCounter(Login);
                }
            }
        }
    }
}