
using DataAsses.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToonKnife.Server;
using ToonKnife.Server.Controllers;
using ToonKnife.Server.DataAsses;
using ToonKnife.Server.Fight;

namespace ToonKnife.TestServer
{
    public class BotTestServer
    {
        UserFightQueue _userFightQueue;
        SettingsStorage _setingsStorage;
        FightList _fightList;

        List<TestBotControllerFactory> _botControllersFactory;

        ToonKnifeTest _test;

        public BotTestServer()
        {
            _test = new ToonKnifeTest();
            ToonKnifeTest.current = _test;
            _test.UserCount = 500;

            _test.Step_AllRabbitUsers_SendFail.overrideValueToSuccess = _test.RabbitTotalCount;
            _test.Step_AllWolfUsers_SendWin.overrideValueToSuccess = _test.WolfTotalCount;

            _botControllersFactory = new List<TestBotControllerFactory>();
            _setingsStorage = new SettingsStorage(new TextFileConfig("ToonKnife"));
            _fightList = new FightList(_setingsStorage);

            _userFightQueue = new UserFightQueue();
            _userFightQueue.UsersReady += UserFightQueue_UserPairReady;
            _userFightQueue.UserEnqueue += UserFightQueue_UserEnqueue;


            for (int i = 0; i < _test.WolfTotalCount; i++)
            {
                {
                    Console.WriteLine(typeof(TestBotFighterController).Name + $"Create boots i={i}");
                    TestBotControllerFactory wolf = new TestBotControllerFactory("TestBot_Wolf_" + i, _userFightQueue, _setingsStorage, true, _test);
                    TestBotMainController wolfController = (TestBotMainController)wolf.CreateMainController();
                    wolfController.GoToFightQueue();

                    _botControllersFactory.Add(wolf);
                }

                {
                    TestBotControllerFactory rabbit = new TestBotControllerFactory("TestBot_Rabbit_" + i, _userFightQueue, _setingsStorage, false, _test);
                    TestBotMainController rabbitController = (TestBotMainController)rabbit.CreateMainController();
                    rabbitController.GoToFightQueue();

                    _botControllersFactory.Add(rabbit);
                }
            }
        }

        public void Stop()
        {
            _test.PrintAllResult();
        }

        public void UpdateLoop()
        {
            Timer.Instanse.UpdateFrame(DateTime.Now);
        }

        void UserFightQueue_UserEnqueue(UserFightQueue sender, UserFightQueue.Entry entry)
        {
            _test.Step_AllUsers_EnqueueInFightQueue.IncrimentCounter(entry.controllersFactory.Login);
        }

        void UserFightQueue_UserPairReady(object sender, UserFightQueue.Entry[] users)
        {
            var fightControillerContainer = _fightList.CreateAndAddNewFight(users);

            fightControillerContainer.fight.KnifeReady += (Fight fight, int index) =>
              {
                  var controller = fightControillerContainer.GetControllerByIndexInFight(index);
                  _test.Step_AllUsers_WaitForFight_And_SeayReadyToFight.IncrimentCounter(controller.Login);
              };

            fightControillerContainer.SendEnemyInfo();
        }
    }
}