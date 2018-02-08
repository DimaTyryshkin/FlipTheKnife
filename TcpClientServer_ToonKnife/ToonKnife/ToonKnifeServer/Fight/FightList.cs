using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ToonKnife.Server.DataAsses;

namespace ToonKnife.Server.Fight
{
    public partial class FightList
    {
        SettingsStorage _setingsStorage;

        List<FightControllersContainer> _fightsList;
         
        public FightList(SettingsStorage setingsStorage)
        {
            _setingsStorage = setingsStorage;

            _fightsList = new List<FightControllersContainer>();
        }

        public FightControllersContainer CreateAndAddNewFight(UserFightQueue.Entry[] fighters)
        {
            FightControllersContainer userFight = new FightControllersContainer(_setingsStorage, fighters, _fightsList.Count);
            userFight.fight.FightClose += Fight_FightClose_Handler;
            userFight.fight.Win += Fight_Win_Handler;
            _fightsList.Add(userFight);
            
            return userFight;
        }

        void Fight_Win_Handler(object sender, int winKnife)
        {
            //UserFighterController c = _fightsList[((Fight)sender).FightIndex]._controllers[winKnife];

            // TODO начислить рейтинг
            // c.User.Login 
        }

        void Fight_FightClose_Handler(Fight fight)
        {
            fight.FightClose -= Fight_FightClose_Handler;
            fight.Win -= Fight_Win_Handler;

            _fightsList.RemoveAll(f => f.FightIndex == fight.FightIndex);
        }
    }
}