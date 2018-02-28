using Assets.game.model.knife;

namespace Assets.game.logic.playground.common.behaviours.replays
{
    /// <summary>
    /// Воспроизводит запись игры.
    /// </summary>
    public class ReplayPlayerBehaviour : LocalGameBehaviourBase
    {
        private readonly IReplayReader m_reader;

        public GameOutOfSync gameOutOfSync { get; set; }
        public InvalidStream invalidStream { get; set; }

        public delegate void GameOutOfSync(float deltaRotation);
        public delegate void InvalidStream(string message);

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

            // бросать нож можно только когда он на земле
            if (code == ReplayCommandCode.ThrowDebug && game.knifeState != KnifeState.Freeze)
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
            if (!m_reader.ReadNext(out time, out code))
            {
                RaiseInvalidStream("Failed to read command");
                return;
            }

            switch (code)
            {
                case ReplayCommandCode.Throw:
                    HandleThrow(game);
                    break;

                case ReplayCommandCode.ThrowDebug:
                    HandleThrowDebug(game);
                    break;

                case ReplayCommandCode.Restart:
                    HandleRestart(game);
                    break;

                case ReplayCommandCode.ChangeMode:
                    HandleChangeMode(game);
                    break;

                default:
                    RaiseInvalidStream("Invalid command code");
                    break;
            }
        }

        private void HandleThrow(ILocalGame game)
        {
            float force;
            if (!m_reader.ReadThrow(out force))
            {
                RaiseInvalidStream("Failed to read throw");
                return;
            }

            game.Throw(force);
        }

        private void HandleThrowDebug(ILocalGame game)
        {
            float force2, startRotation;
            if (!m_reader.ReadThrowDebug(out force2, out startRotation))
            {
                RaiseInvalidStream("Failed to read throw debug");
                return;
            }

            if (game.knifeRotation != startRotation)
            {
                RaiseGameOutOfSync(game.knifeRotation - startRotation);
            }

            game.Throw(force2);
        }

        private void HandleRestart(ILocalGame game)
        {
            if (!m_reader.ReadRestart())
            {
                RaiseInvalidStream("Failed to read restart");
                return;
            }

            game.Restart();
        }

        private void HandleChangeMode(ILocalGame game)
        {
            KnifeMode knifeMode;
            if (!m_reader.ReadChangeMode(out knifeMode))
            {
                RaiseInvalidStream("Failed to read change speed");
                return;
            }

            game.SetKnifeMode(knifeMode);
        }

        private void RaiseInvalidStream(string message)
        {
            if (invalidStream != null)
                invalidStream(message);
        }

        private void RaiseGameOutOfSync(float deltaRotation)
        {
            if (gameOutOfSync != null)
                gameOutOfSync(deltaRotation);
        }
    }
}