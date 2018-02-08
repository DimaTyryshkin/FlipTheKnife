using System;
using System.Collections.Generic;
using System.Linq;
using ToonKnife.Server.DataAsses;

namespace ToonKnife.Server.Fight
{

    /// <summary>
    /// Эта штука объедиянет модель <see cref="Fight"/> и средства ввода в эту модель данных <see cref="IFighterController"/>, поступающих от юзеров\ботов
    /// </summary>
    public class FightControllersContainer
    {
        public Fight fight;

        List<IFighterController> _controllers;
        Fight.KnifeInfo[] _knifesInfo;
        SettingsStorage _setingsStorage;

        public int FightIndex { get; }

        public FightControllersContainer(SettingsStorage setingsStorage, UserFightQueue.Entry[] fighters, int fightIndex)
        {
            if (fighters == null)
            {
                throw new ArgumentNullException(nameof(fighters));
            }

            _setingsStorage = setingsStorage ?? throw new ArgumentNullException(nameof(setingsStorage));
            FightIndex = fightIndex;
            _controllers = new List<IFighterController>();


            _knifesInfo = fighters.Select(f => new Fight.KnifeInfo(f.knifeName, f.knifeMode)).ToArray();

            fight = new Fight(_knifesInfo, _setingsStorage, fightIndex);

            for (int i = 0; i < fighters.Length; i++)
            {
                UserFightQueue.Entry f = fighters[i];

                IFighterController controller = f.controllersFactory.CreateFighterController(fight, i);

                _controllers.Add(controller);
            } 
        }

        public void SendEnemyInfo()
        {
            //Посылаем информацию о враге
            for (int i = 0; i < _controllers.Count; i++)
            {
                var enemyIndex = 1 - i;//для случая когода 2 игрока

                _controllers[i].SendFightCreated
                    (
                    _controllers[enemyIndex].Login,
                    _knifesInfo[enemyIndex].knifeName,
                    _knifesInfo[enemyIndex].knifeMode
                    );
            }
        }

        public IFighterController GetControllerByIndexInFight(int knifeIndexInFight)
        {
            return _controllers[knifeIndexInFight];
        }
    }
}