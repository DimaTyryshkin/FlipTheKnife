
namespace Assets.game.logic.playground.common.behaviours
{
    /// <summary>
    /// Дополнительное поведение игры, 
    /// перезапускает игру после неудачного приземления.
    /// </summary>
    public class RestartOnFailBehaviour : LocalGameBehaviourBase
    {
        private readonly float m_restartTimeout;

        private float? m_timeLeftToRestart;

        /// <summary>
        /// Создаёт новый экземпляр класса.
        /// </summary>
        /// <param name="restartTimeout">Количество секунд, через которое необходимо перезапустить игру после неудачного приземления.</param>
        public RestartOnFailBehaviour(float restartTimeout)
        {
            m_restartTimeout = restartTimeout;
        }

        public override void PostFail(ILocalGame game)
        {
            base.PostFail(game);

            m_timeLeftToRestart = m_restartTimeout;
        }

        public override bool PreUpdate(ILocalGame game, float deltaTime)
        {
            if (m_timeLeftToRestart.HasValue)
            {
                m_timeLeftToRestart -= deltaTime;
                if (m_timeLeftToRestart <= 0)
                {
                    m_timeLeftToRestart = null;
                    game.Restart();
                }
            }

            return base.PreUpdate(game, deltaTime);
        }
    }
}