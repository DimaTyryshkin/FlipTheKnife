
using Assets.game.model.knife;

namespace Assets.game.logic.playground.common.behaviours.replays
{
    /// <summary>
    /// Записывает игру.
    /// </summary>
    public class ReplayRecorderBehaviour : LocalGameBehaviourBase
    {
        private readonly IReplayWriter m_writer;
        private readonly bool m_debug;

        public ReplayRecorderBehaviour(IReplayWriter replayWriter, bool debug = false)
        {
            m_writer = replayWriter;
            m_debug = debug;
        }

        public override bool PreThrow(ILocalGame game, float force)
        {
            if (m_debug)
            {
                m_writer.WriteThrowDebug(game.time, force, game.knifeRotation);
            }
            else
            {
                m_writer.WriteThrow(game.time, force);
            }

            return true;
        }

        public override bool PreRestart(ILocalGame game)
        {
            m_writer.WriteRestart(game.time);

            return base.PreRestart(game);
        }

        public override bool PreChangeKnifeMode(ILocalGame game, KnifeMode knifeMode)
        {
            m_writer.WriteChangeMode(game.time, knifeMode);

            return base.PreChangeKnifeMode(game, knifeMode);
        }
    }
}