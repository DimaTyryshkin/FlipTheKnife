using System;
using Assets.game.model.knife;

namespace Assets.game.logic.playground.common.adapters
{
    /// <summary>
    /// Адаптер старой реализации игры к интерфейсу IGame.
    /// </summary>
    /// <remarks>
    /// Класс Game должен реализовывать интерфейс IGame,
    /// но для этого необходимо полность переписать Game,
    /// что на данный момент затруднительно.
    /// </remarks>
    public class GameAdapter : ILocalGame, IDisposable
    {
        private readonly Game m_game;
        private readonly ILocalGameBehaviour[] m_behaviours;

        private readonly string m_knife;
        private readonly string m_level;

        public float time { get { return m_game.time; } }
        public float knifePosition { get { return m_game.knife.position; } }
        public float knifeRotation { get { return m_game.knife.rotation; } }
        public string knife { get { return m_game.knifeDef.key; } }
        public string level { get { return m_level; } }
        public int score { get { return m_game.score; } }
        public KnifeMode knifeMode { get { return m_game.knifeMode; } }
        public KnifeState knifeState { get { return m_game.knife.state; } }

        public event GameKnifeModeChanged modeChanged;
        public event GameKnifeStateChanged stateChanged;
        public event GameKnifeFlip flip;
        public event GameKnifeReset knifeReset;
        public event GameKnifeThrowing knifeThrowing;
        public event GameKnifeThrowFail knifeThrowFail;
        public event GameKnifeThrowSuccess knifeThrowSuccess;

        public GameAdapter(Game game, string knife, string level, ILocalGameBehaviour[] behaviours)
        {
            m_game = game;
            m_knife = knife;
            m_level = level;
            m_behaviours = behaviours;

            m_game.knifeModeChanged += OnKnifeModeChanged;
            m_game.knife.stateChanged += OnKnifeStateChanged;
            m_game.knife.onFlip += OnKnifeFlip;
            m_game.knife.onReset += OnKnifeReset;
            m_game.knife.throwing += OnKnifeThrowing;
            m_game.onThrowSuccess += OnKnifeThrowSuccess;
            m_game.knife.throwFail += OnKnifeThrowFail;            
        }

        public void Dispose()
        {
            m_game.knifeModeChanged -= OnKnifeModeChanged;
            m_game.knife.stateChanged -= OnKnifeStateChanged;
            m_game.knife.onFlip -= OnKnifeFlip;
            m_game.knife.onReset -= OnKnifeReset;
            m_game.knife.throwing -= OnKnifeThrowing;
            m_game.onThrowSuccess -= OnKnifeThrowSuccess;
            m_game.knife.throwFail -= OnKnifeThrowFail;

            var disposable = m_game as IDisposable;
            if (disposable != null)
                disposable.Dispose();
        }

        public void Update(float deltaTime)
        {
            for (int i = 0; i < m_behaviours.Length; i++)
            {
                if (!m_behaviours[i].PreUpdate(this, deltaTime))
                    return;
            }

            m_game.Update(deltaTime);
        }

        public void SetKnifeMode(KnifeMode knifeMode)
        {
            for (int i = 0; i < m_behaviours.Length; i++)
            {
                if (!m_behaviours[i].PreChangeKnifeMode(this, knifeMode))
                    return;
            }

            m_game.SetKnifeMode(m_knife, knifeMode);
        }

        public void Restart()
        {
            for (int i = 0; i < m_behaviours.Length; i++)
            {
                if (!m_behaviours[i].PreRestart(this))
                    return;
            }

            m_game.Restart();
        }

        public void Throw(float force)
        {
            for (int i = 0; i < m_behaviours.Length; i++)
            {
                if (!m_behaviours[i].PreThrow(this, force))
                    return;
            }

            m_game.Throw(force);
        }

        private void OnKnifeStateChanged(KnifeState obj)
        {
            if (stateChanged != null) stateChanged(obj);
        }

        private void OnKnifeModeChanged(KnifeMode obj)
        {
            if (modeChanged != null) modeChanged(obj);
        }

        private void OnKnifeThrowing()
        {
            if (knifeThrowing != null) knifeThrowing();
        }

        private void OnKnifeReset()
        {
            if (knifeReset != null) knifeReset();
        }

        private void OnKnifeFlip()
        {
            if (flip != null) flip();
        }

        private void OnKnifeThrowFail(float rotationSpeed, float deltaAngle)
        {
            for (int i = 0; i < m_behaviours.Length; i++)
            {
                m_behaviours[i].PostFail(this);
            }

            if (knifeThrowFail != null)
            {
                FailInfo failInfo;
                failInfo.rotationSpeed = rotationSpeed;
                failInfo.deltaAngles = deltaAngle;
                knifeThrowFail(failInfo);
            }
        }

        private void OnKnifeThrowSuccess(ThrowInfo info)
        {
            if (knifeThrowSuccess != null) knifeThrowSuccess(info);
        }
    }
}