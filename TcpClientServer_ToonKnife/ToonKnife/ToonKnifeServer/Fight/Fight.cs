using Assets.game.logic.playground.common;
using Assets.game.model.knife;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToonKnife.Server.DataAsses;

namespace ToonKnife.Server.Fight
{
    public class Fight
    {
        public struct KnifeInfo
        {
            public string knifeName;
            public KnifeMode knifeMode;

            public KnifeInfo(string knifeName, KnifeMode knifeMode)
            {
                this.knifeName = knifeName ?? throw new ArgumentNullException(nameof(knifeName));
                this.knifeMode = knifeMode;
            }
        }

        //---fields
        GameLoop[] games;
        DateTime timeEndFight;
        int _knifesReadyCounter;

        //---prop
        public int FightIndex { get; }
        public bool WaitForReady { get; private set; }

        //---events
        public event EventHandler<int> Win;

        /// <summary>
        /// минимум 2 победителя(ничья)
        /// </summary>
        public event Action<Fight> DeatHead;
        public event EventHandler<KnifeThrowEventArgs> KnifeThrow;
        public event Action<Fight> FightClose;
        public event Action<Fight> FightStart;

        public Fight(KnifeInfo[] knifes, SettingsStorage knifeDefStorage, int fightIndex)
        {
            WaitForReady = true;

            FightIndex = fightIndex;
            games = new GameLoop[knifes.Length];
            _knifesReadyCounter = knifes.Length;

            for (int i = 0; i < knifes.Length; i++)
            {
                Game g = new Game(knifeDefStorage.KnifeSettings, knifeDefStorage.Settings);

                games[i] = new GameLoop(g, timeEndFight);
            }
        }

        public bool CanThrow(int knifeIndex)
        {
            GameLoop gameLoop = games[knifeIndex];

            return !WaitForReady && gameLoop.CanThrow;
        }

        public void SetOneKnifeReady()
        {
            _knifesReadyCounter--;

            if (_knifesReadyCounter == 0)
            {
                StartFight();
                WaitForReady = false;
            }
        }

        void StartFight()
        {
            timeEndFight = DateTime.Now + TimeSpan.FromSeconds(60);
            Timer.AddTimer(FightEnd_Timer, timeEndFight);

            FightStart?.Invoke(this);
        }

        internal void ThrowKnife(int knifeIndex, float input)
        {
            GameLoop gameLoop = games[knifeIndex];

            if (gameLoop.CanThrow)
            {
                gameLoop.Throw(input);
                KnifeThrow?.Invoke(this, new KnifeThrowEventArgs(knifeIndex, gameLoop.TimeThrow, gameLoop.TimeNextThrow, input));
            }
        }

        void FightEnd_Timer()
        {
            int maxScore = -1;
            int indexMax = -1;

            // нахожим индекс победителя
            for (int i = 0; i < games.Length; i++)
            {
                if (games[i].ScoreInFight > maxScore)
                {
                    maxScore = games[i].ScoreInFight;
                    indexMax = i;
                }
            }

            // проверка на ничью
            bool deadHeat = false;//ничья

            for (int i = 0; i < games.Length; i++)
            {
                if (i != indexMax && games[i].ScoreInFight == maxScore)
                {
                    deadHeat = true;
                    break;
                }
            }

            if (deadHeat)
            {
                DeatHead?.Invoke(this);
            }
            else
            {
                Win?.Invoke(this, indexMax);
            }

            FightClose?.Invoke(this);
        }
    }
}