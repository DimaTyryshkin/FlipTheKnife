
using Assets.game.model.knife;

namespace Assets.game.logic.playground.common.behaviours.replays
{
    /// <summary>
    /// Записывает игру.
    /// </summary>
    public class ReplayRecorderBehaviour : LocalGameBehaviourBase
    {
        private readonly IReplayWriter m_writer;

        public ReplayRecorderBehaviour(IReplayWriter replayWriter)
        {
            m_writer = replayWriter;
        }

        public override bool PreThrow(ILocalGame game, float force)
        {
            m_writer.WriteThrow(game.time, force);

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