using Assets.game.model.knife;

namespace Assets.game.logic.playground.common
{
    /// <summary>
    /// Интерфейс игры, управляемой локально.
    /// </summary>
    public interface ILocalGame : IGame
    {
        /// <summary>
        /// Изменяет режим ножа.
        /// </summary>
        /// <param name="knifeMode">Новый режим ножа.</param>
        void SetKnifeMode(KnifeMode knifeMode);

        /// <summary>
        /// Запускает (перезапускает) игру.
        /// </summary>
        void Restart();

        /// <summary>
        /// Бросает нож с заданой силой.
        /// </summary>
        /// <param name="force">Сила с которой необходимо бросить нож.</param>
        void Throw(float force);
    }
}