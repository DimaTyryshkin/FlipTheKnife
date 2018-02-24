
using DataAsses.Config;
using ScsService.Server;
using ScsService.Server.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToonKnife.Server;
using ToonKnife.Server.Controllers;
using ToonKnife.Server.DataAsses;
using ToonKnife.Server.Fight;
using ToonKnife.Server.ScsServiceAdapter.Controllers;

namespace ToonKnife.TestServer
{
    public class BotTestServer
    {
        ToonKnifeTest _test;
        Server.Server _server;
        private ScsService.Server.ScsService _scsServer;

        public BotTestServer(int serverPort, int wolfCount, int serverSideBotsCount = 0)
        {
            _test = new ToonKnifeTest();
            ToonKnifeTest.current = _test;
            _test.UserCount = wolfCount * 2;

            //TODO как-то надо протестить много каток подряд с одного лкиента без переподключения
            _test.Step_AllRabbitUsers_SendFail.overrideValueToSuccess = _test.RabbitTotalCount;
            _test.Step_AllWolfUsers_SendWin.overrideValueToSuccess = _test.WolfTotalCount;

            _server = new Server.Server();

            _server.UserFightQueue.UserEnqueue += UserFightQueue_UserEnqueue;
            _server.FightList.FightCreated += FightList_FightCreated;

            // scs
            _scsServer = new ScsService.Server.ScsService(serverPort);
            _scsServer.OnUserLogin += ScsServer_OnUserLogin;
            _scsServer.OnUserDisconnected += ScsServer_OnUserDisconnected;
            _scsServer.Start();

            for (int i = 0; i < serverSideBotsCount; i++)
            {

            }
        }

        public void Stop()
        {
            _scsServer.Stop();
            _server.Stop();
            _test.PrintAllResult();
        }

        public void UpdateLoop()
        {
            _scsServer.MainLoopFrame();
            _server.UpdateLoop();
        }



        void ScsServer_OnUserLogin(object sender, UserEventArgs e)
        {
            Console.WriteLine(typeof(BotTestServer).Name + $" Login={e.User.Login} userLogin");
            UserControllerFactory userControllerFactory = new UserControllerFactory(e.User);
            _server.AddUser(userControllerFactory);
        }

        void ScsServer_OnUserDisconnected(object sender, UserEventArgs e)
        {
            _server.RemoveUser(e.User.Login);
        }



        void UserFightQueue_UserEnqueue(UserFightQueue sender, UserFightQueue.Entry entry)
        {
            Console.WriteLine(typeof(BotTestServer).Name + $" Login={entry.controllersFactory.Login} FightQueue_UserEnqueue");
            _test.Step_AllUsers_EnqueueInFightQueue.IncrimentCounter(entry.controllersFactory.Login);
        }

        void FightList_FightCreated(FightList fightList, FightControllersContainer fightControllersContainer)
        {
            fightControllersContainer.fight.KnifeReady += (Fight fight, int index) =>
            {
                var controller = fightControllersContainer.GetControllerByIndexInFight(index);
                _test.Step_AllUsers_WaitForFight_And_SeayReadyToFight.IncrimentCounter(controller.Login);
            };

            fightControllersContainer.fight.Win += (object fight, int winner) =>
              { 
                  {
                      var winnderController = fightControllersContainer.GetControllerByIndexInFight(winner);

                      Console.WriteLine("Winer " + winnderController.Login);

                      if (winnderController.Login.Contains("Wolf"))
                      {
                          _test.Step_AllWolfUsers_SendWin.IncrimentCounter(winnderController.Login);
                      }
                  }

                  {
                      var loserController = fightControllersContainer.GetControllerByIndexInFight(1 - winner);
                      if (loserController.Login.Contains("Rabbit"))
                      {
                          _test.Step_AllRabbitUsers_SendFail.IncrimentCounter(loserController.Login);
                      }
                  }
              };
        }
    }
}