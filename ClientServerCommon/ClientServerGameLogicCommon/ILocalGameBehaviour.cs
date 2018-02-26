
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
        void PostFail(ILocalGame game);
    }
}