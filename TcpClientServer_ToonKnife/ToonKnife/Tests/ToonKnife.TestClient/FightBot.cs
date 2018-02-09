using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToonKnife.Common;
using ToonKnife.Server;

namespace ToonKnife.TestClient
{
    public class FightBot
    {
        float[] _input;
        int curInputIndex = 0;

        bool _isWolf;
        bool _waitForReady;
        bool _fightIsClosed;
        FightController _fightController;


        public string Login { get; }

        public FightBot(string login, bool isWolf, FightController fightController)
        {
            Login = login;
            _isWolf = isWolf;
            _fightController = fightController;
            _fightController.FightStart += FightController_FightStart;
            _fightController.KnifeThrow += FightController_KnifeThrow;
            _fightController.FightEnd += FightController_FightEnd;


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
                    1068f
                    };
            }
        }

        private void FightController_FightEnd(int winer)
        {
            _fightIsClosed = true;
        }

        void FightController_KnifeThrow(KnifeThrowEventArg knifeThrowEventArg)
        {
            if (_fightController.KnifeIndex == knifeThrowEventArg.KnifeId)
            {
                Timer.AddTimer(Thorow, knifeThrowEventArg.TimeNextThrow + TimeSpan.FromSeconds(1));
            }
        }

        void FightController_FightStart()
        {
            Thorow();
        }

        void Thorow()
        {
            if (!_fightIsClosed)
            {
                float input = _input[curInputIndex];
                curInputIndex = (curInputIndex + 1) % _input.Length;

                Console.WriteLine(typeof(FightBot).Name + $" Login={Login} throw knife");
                _fightController.SendThrow(input);
            }
        }
    }
}