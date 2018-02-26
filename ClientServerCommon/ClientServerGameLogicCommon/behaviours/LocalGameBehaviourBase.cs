
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
        public virtual void PostFail(ILocalGame game)
        {

        }

        public virtual bool PreUpdate(ILocalGame game, float deltaTime)
        {
            return true;
        }
    }
}