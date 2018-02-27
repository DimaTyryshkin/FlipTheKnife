using Assets.game.model.knife;

namespace Assets.game.logic.playground.common
{
    /// <summary>
    /// Интерфейс игры.
    /// </summary>
    public interface IGame
    {
        /// <summary>
        /// Выбранный нож.
        /// </summary>
        string knife { get; }

        /// <summary>
        /// Выбранный фон.
        /// </summary>
        string level { get; }

        /// <summary>
        /// Набранные очки.
        /// </summary>
        int score { get; }

        /// <summary>
        /// Количество секунд с начала игры.
        /// </summary>
        float time { get; }

        /// <summary>
        /// Текущий режим ножа.
        /// </summary>
        KnifeMode knifeMode { get; }

        /// <summary>
        /// Состояние ножа.
        /// </summary>
        KnifeState knifeState { get; }

        /// <summary>
        /// Текущий поаорот ножа.
        /// </summary>
        float knifeRotation { get; }

        /// <summary>
        /// Текущая позиция ножа.
        /// </summary>
        float knifePosition { get; }

        /// <summary>
        /// Обновляет состояние игры.
        /// </summary>
        /// <param name="deltaTime">Количество секунд, прошедших с предыдущего обновления игры.</param>
        void Update(float deltaTime);

        /// <summary>
        /// Вызывается при смене режима ножа.
        /// </summary>
        event GameKnifeModeChanged modeChanged;

        /// <summary>
        /// Вызывается при изменении состояния ножа.
        /// </summary>
        event GameKnifeStateChanged stateChanged;

        /// <summary>
        /// Вызывается при каждом полном обороте ножа в воздухе.
        /// </summary>
        event GameKnifeFlip flip;

        /// <summary>
        /// Вызывается при сбросе состояния ножа.
        /// </summary>
        event GameKnifeReset knifeReset;

        /// <summary>
        /// Происходит при выбрасывании ножа.
        /// </summary>
        event GameKnifeThrowing knifeThrowing;

        /// <summary>
        /// Происходит при неудачном приземлении ножа.
        /// </summary>
        event GameKnifeThrowFail knifeThrowFail;

        /// <summary>
        /// Проиходит при удачном втыкании ножа.
        /// </summary>
        event GameKnifeThrowSuccess knifeThrowSuccess;
    }

    public delegate void GameKnifeModeChanged(KnifeMode knifeMode);
    public delegate void GameKnifeStateChanged(KnifeState knifeState);
    public delegate void GameKnifeFlip();
    public delegate void GameKnifeReset();
    public delegate void GameKnifeThrowing();
    public delegate void GameKnifeThrowFail(FailInfo failInfo);
    public delegate void GameKnifeThrowSuccess(ThrowInfo throwInfo);
}