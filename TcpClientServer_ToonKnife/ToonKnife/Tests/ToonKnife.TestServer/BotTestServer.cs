
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
        ToonKnifeTest _test;
        Server.Server _server;

        public BotTestServer()
        {
            _test = new ToonKnifeTest();
            ToonKnifeTest.current = _test;
            _test.UserCount = 500;

            _test.Step_AllRabbitUsers_SendFail.overrideValueToSuccess = _test.RabbitTotalCount;
            _test.Step_AllWolfUsers_SendWin.overrideValueToSuccess = _test.WolfTotalCount;

            _server = new Server.Server();

            _server.UserFightQueue.UserEnqueue += UserFightQueue_UserEnqueue;
            _server.FightList.FightCreated += FightList_FightCreated;


            for (int i = 0; i < _test.WolfTotalCount; i++)
            {
                {
                    Console.WriteLine(typeof(TestBotFighterController).Name + $"Create boots i={i}");
                    TestBotControllerFactory wolf = new TestBotControllerFactory("TestBot_Wolf_" + i, true, _test);
                    _server.AddUser(wolf);
                }

                {
                    TestBotControllerFactory rabbit = new TestBotControllerFactory("TestBot_Rabbit_" + i, false, _test);
                    _server.AddUser(rabbit);
                }
            }
        }

        private void FightList_FightCreated(FightList fightList, FightControllersContainer fightControllersContainer)
        {
            fightControllersContainer.fight.KnifeReady += (Fight fight, int index) =>
            {
                var controller = fightControllersContainer.GetControllerByIndexInFight(index);
                _test.Step_AllUsers_WaitForFight_And_SeayReadyToFight.IncrimentCounter(controller.Login);
            };
        }

        public void Stop()
        {
            _server.Stop();
            _test.PrintAllResult();
        }

        public void UpdateLoop()
        {
            _server.UpdateLoop();
        }

        void UserFightQueue_UserEnqueue(UserFightQueue sender, UserFightQueue.Entry entry)
        {
            _test.Step_AllUsers_EnqueueInFightQueue.IncrimentCounter(entry.controllersFactory.Login);
        }
    }
}