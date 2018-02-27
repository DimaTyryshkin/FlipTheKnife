
using Assets.game.model.knife;

namespace Assets.game.logic.playground.common.behaviours.replays
{
    /// <summary>
    /// Воспроизводит запись игры.
    /// </summary>
    public class ReplayPlayerBehaviour : LocalGameBehaviourBase
    {
        private readonly IReplayReader m_reader;

        public ReplayPlayerBehaviour(IReplayReader replayReader)
        {
            m_reader = replayReader;
        }

        public override bool PreUpdate(ILocalGame game, float deltaTime)
        {
            float time;
            ReplayCommandCode code;
            if (m_reader.PeekNext(out time, out code) && time <= game.time)
            {
                if (CanHandleCommand(game, code))
                {
                    HandleReplayCommand(game);
                }
            }

            return true;
        }

        private bool CanHandleCommand(ILocalGame game, ReplayCommandCode code)
        {
            // Из-за пинга события могут приходить раньше времени,
            // в этом случае мы не должны запускать событие в неподходящий момент.

            // бросать нож можно только когда он на земле
            if (code == ReplayCommandCode.Throw && game.knifeState != KnifeState.Freeze)
                return false;

            // нельзя перезапускать игру во время полёта ножа
            if (code == ReplayCommandCode.Restart && (game.knifeState == KnifeState.Falling || game.knifeState == KnifeState.Flying))
                return false;

            // изменчть скорость модно только когда нож воткнут в землю
            if (code == ReplayCommandCode.ChangeMode && game.knifeState != KnifeState.Freeze)
                return false;

            return true;
        }

        private void HandleReplayCommand(ILocalGame game)
        {
            float time;
            ReplayCommandCode code;
            if (false == m_reader.ReadNext(out time, out code))
                return;

            switch (code)
            {
                case ReplayCommandCode.Throw:
                    float force;
                    if (m_reader.ReadThrow(out force))
                    {
                        game.Throw(force);
                    }
                    break;

                case ReplayCommandCode.Restart:
                    if (m_reader.ReadRestart())
                    {
                        game.Restart();
                    }
                    break;

                case ReplayCommandCode.ChangeMode:
                    KnifeMode knifeMode;
                    if (m_reader.ReadChangeMode(out knifeMode))
                    {
                        game.SetKnifeMode(knifeMode);
                    }
                    break;
            }
        }
    }
}