using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToonKnife.Server.DataAsses;

namespace ToonKnife.Server.Fight
{
    public class FightList
    {
        public class UserVsUserFight
        {
            public Fight fight;
            public List<UserFighterController> controllers;

            public UserVsUserFight(Fight fight)
            {
                this.fight = fight ?? throw new ArgumentNullException(nameof(fight));

                controllers = new List<UserFighterController>();
            }
        }



        SettingsStorage _setingsStorage;

        List<UserVsUserFight> fightsList;



        public FightList(SettingsStorage setingsStorage)
        {
            _setingsStorage = setingsStorage;

            fightsList = new List<UserVsUserFight>();
        }

        public UserVsUserFight CreateNewFight(UserFightQueue.Entry[] fighters)
        {
            var knifesInfo = fighters.Select(f => new Fight.KnifeInfo(f.knifeName, f.knifeMode)).ToArray();

            Fight fight = new Fight(knifesInfo, _setingsStorage, fightsList.Count);

            UserVsUserFight usersFight = new UserVsUserFight(fight);

            for (int i = 0; i < fighters.Length; i++)
            {
                UserFightQueue.Entry f = fighters[i];

                UserFighterController controller = new UserFighterController(f.user, fight, i);


                usersFight.controllers.Add(controller);
            }

            //Посылаем информацию о враге
            for (int i = 0; i < usersFight.controllers.Count; i++)
            {
                var enemyIndex = 1 - i;//для случая когода 2 игрока

                usersFight.controllers[i].SendFightCreated
                    (
                    usersFight.controllers[enemyIndex].User.Login,
                    knifesInfo[enemyIndex].knifeName,
                    knifesInfo[enemyIndex].knifeMode
                    );
            }

            return usersFight;
        }

        public void AddFight(UserVsUserFight userFight)
        {
            userFight.fight.FightClose += Fight_FightClose_Handler;
            userFight.fight.Win += Fight_Win;

            fightsList.Add(userFight);
        }



        void Fight_Win(object sender, int winKnife)
        {
            UserFighterController c = fightsList[((Fight)sender).FightIndex].controllers[winKnife];

            // TODO начислить рейтинг
            // c.User.Login 
        }

        void Fight_FightClose_Handler(Fight fight)
        {
            fight.FightClose -= Fight_FightClose_Handler;
            fight.Win -= Fight_Win;

            fightsList.RemoveAt(fight.FightIndex);
        }
    }
}