namespace Assets.game.logic.playground.common.behaviours
{
    /// <summary>
    /// Вызывает коллбеки при вызове методов игры.
    /// </summary>
    public class CallbacksBehaviour : LocalGameBehaviourBase
    {
        public GameKnifeReset restart;

        public override bool PreRestart(ILocalGame game)
        {
            if (restart != null)
                restart();

            return base.PreRestart(game);
        }
    }
}