using Assets.game.model.knife;

namespace Assets.game.logic.playground.common
{
    /// <summary>
    /// Интерфейс дополняющий поведение игры.
    /// </summary>
    /// <remarks>
    /// Применяется при необходимости добавить новое поведение игре.
    /// </remarks>
    public interface ILocalGameBehaviour
    {
        /// <summary>
        /// Вызывется перед обновлением игры.
        /// </summary>
        /// <param name="game">Игра, поведение которой дополняется.</param>
        /// <param name="deltaTime">Количество секунд, прошедших с предыдущего обновления игры.</param>
        /// <returns>false если небходимо прекратить обновление игры, иначе true</returns>
        bool PreUpdate(ILocalGame game, float deltaTime);

        /// <summary>
        /// Вызывается после неудачного приземления ножа.
        /// </summary>
        /// <param name="game">Игра, поведение которой дополняется.</param>
        void PostKnifeThrowFail(ILocalGame game, FailInfo failInfo);

        /// <summary>
        /// Вызывается перед броском ножа.
        /// </summary>
        /// <param name="game">Игра, поведение которой дополняется.</param>
        /// <param name="force">Сила, с которой будет брошен нож.</param>
        /// <returns>false если необходимо отменить бросок, иначе true</returns>
        bool PreThrow(ILocalGame game, float force);

        /// <summary>
        /// Вызывается перед сменой режима ножа.
        /// </summary>
        /// <param name="game">Игра, поведение которой дополняется.</param>
        /// <param name="knifeMode">Новый режим ножа.</param>
        /// <returns>false если необходимо отменить смену скорости, иначе true</returns>
        bool PreChangeKnifeMode(ILocalGame game, KnifeMode knifeMode);

        /// <summary>
        /// Вызывается перед сбросом состояния игры.
        /// </summary>
        /// <param name="game">Игра, поведение которой дополняется.</param>
        /// <returns>false если необходимо отменить сброс состояния, иначе true</returns>
        bool PreRestart(ILocalGame game);

        void PostKnifeStateChnaged(ILocalGame game, KnifeState knifeState);
        void PostKnifeModeChnaged(ILocalGame game, KnifeMode knifeMode);
        void PostKnifeThrowing(ILocalGame game);
        void PostKnifeRestart(ILocalGame game);
        void PostKnifeFlip(ILocalGame game);
        void PostKnifeThrowSuccess(ILocalGame game, ThrowInfo info);
        void PostUpdate(ILocalGame game, float deltaTime);
    }
}