
using Assets.game.model.knife;

namespace Assets.game.logic.playground.common.behaviours
{
    /// <summary>
    /// Базовый класс дополняющих поведений игры.
    /// </summary>
    /// <remarks>
    /// Данный класс не добавляет дополнительного поведения,
    /// а только является реализацией ILocalGameBehaviour с виртуальными методами,
    /// тем самым позволяя переопределять только необходимые методы,
    /// а не все, как это требуется при реализация интерфейса.
    /// </remarks>
    public abstract class LocalGameBehaviourBase : ILocalGameBehaviour
    {
        public virtual void PostKnifeFlip(ILocalGame game)
        { }

        public virtual void PostKnifeModeChnaged(ILocalGame game, KnifeMode knifeMode)
        { }

        public virtual void PostKnifeRestart(ILocalGame game)
        { }

        public virtual void PostKnifeStateChnaged(ILocalGame game, KnifeState knifeState)
        { }

        public virtual void PostKnifeThrowFail(ILocalGame game, FailInfo failInfo)
        { }

        public virtual void PostKnifeThrowing(ILocalGame game)
        { }

        public virtual void PostKnifeThrowSuccess(ILocalGame game, ThrowInfo info)
        { }

        public virtual void PostUpdate(ILocalGame game, float deltaTime)
        { }

        public virtual bool PreChangeKnifeMode(ILocalGame game, KnifeMode knifeMode)
        {
            return true;
        }

        public virtual bool PreRestart(ILocalGame game)
        {
            return true;
        }

        public virtual bool PreThrow(ILocalGame game, float force)
        {
            return true;
        }

        public virtual bool PreUpdate(ILocalGame game, float deltaTime)
        {
            return true;
        }
    }
}