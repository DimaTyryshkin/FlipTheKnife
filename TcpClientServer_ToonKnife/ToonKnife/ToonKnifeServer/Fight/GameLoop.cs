using Assets.game.logic.playground.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToonKnife.Server.Fight
{
    public class GameLoop
    {
        Game _game;
        bool _isUpdate;
        int _scoreInFight;

        public DateTime TimeEndFight = new DateTime(2000,1,1);
        DateTime _timeNextThrow;
        DateTime _timeThrow;


        //---prop
        public bool CanThrow => DateTime.Now >= _timeNextThrow &&
             DateTime.Now <= TimeEndFight &&
            _game.knife.state == Knife.State.Freeze ;

        public int ScoreInFight => _scoreInFight;

        public DateTime TimeThrow   => _timeThrow;

        public DateTime TimeNextThrow  => _timeNextThrow; 


        //---methods
        public GameLoop(Game game)
        {
            _game = game ?? throw new ArgumentNullException(nameof(game));
             
            _timeNextThrow = DateTime.Now;
            _game.knife.throwFail += Knife_throwFail;
            _game.onThrowSuccess += Game_onThrowSuccess;
        }

        public void Throw(float input)
        {
            if (CanThrow)
            {
                _isUpdate = true;
                _timeThrow = DateTime.Now;

                _game.Throw(input);


                float time = 0;
                float timeStep = 0.2f;
                while (_isUpdate)
                {
                    if (time > 5f)
                    {
                        throw new InvalidOperationException(typeof(GameLoop).Name + " : Цикл длится больше 5 секунд");
                    }

                    time += timeStep;
                    _game.Update(timeStep);
                }
            }
        }

        void StopUpdateLoop()
        {
            _isUpdate = false;

            float totalTime = _game.knife.KnifeTrajectory.lastFrame.timeAfterThrow;

            _timeNextThrow = _timeThrow + TimeSpan.FromSeconds(totalTime);
        }

        void Game_onThrowSuccess(ThrowInfo info)
        {
            StopUpdateLoop();

            if (_timeNextThrow < TimeEndFight)
            {
                _scoreInFight += info.score;
            }
        }

        void Knife_throwFail(float deltaAngle)
        {
            StopUpdateLoop();

            _game.Restart();
        }
    }
}