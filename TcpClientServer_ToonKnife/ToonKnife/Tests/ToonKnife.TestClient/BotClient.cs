using Assets.game.model.knife;
using ClientServerGameLogicCommon;
using Hik.Communication.Scs.Server;
using ScsService.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToonKnife.Client.ScsServiceAdapter;
using ToonKnife.Server;

namespace ToonKnife.TestClient
{
    public class BotClient
    {
        private ScsClient _client;
        MainServerController _mainServerController;
        FightBot _bot;
        bool _isWolf;
        int _gamesCount;

        public BotClient(int serverPort, string serverIp, string userName, bool isWolf, int gamesCount)
        {
            _client = new ScsClient(userName, serverIp, serverPort);
            _client.OnUserLogin += Client_OnUserLogin;
            _isWolf = isWolf;
            _gamesCount = gamesCount;

            _client.Connect();
        }

        private void Client_OnUserLogin()
        {
            Console.WriteLine(typeof(BotClient).Name + $" Login={_client.Login} userLogin");

            _mainServerController = new MainServerController(_client);
            _mainServerController.FigthCreated += MainServerController_FigthCreated;

            GoToFightQueue();
        }

        void GoToFightQueue()
        {
            Console.WriteLine(typeof(BotClient).Name + $" Login={_client.Login} created");
            _mainServerController.GoToFightQueue("knife00", KnifeMode.Medium);
        }

        private void MainServerController_FigthCreated(MainServerController mainServerController, FightController fightController, EnemyDef enemyDef)
        {
            fightController.FightEnd += FightController_FightEnd;
            _bot = new FightBot(_client.Login, _isWolf, fightController);

            fightController.KnifeThrow += FightController_KnifeThrow;
            fightController.SendReady();
        }

        private void FightController_KnifeThrow(Common.KnifeThrowEventArg obj)
        {
            Console.WriteLine("FightController_KnifeThrow knifeId=" + obj.KnifeId);
        }

        void FightController_FightEnd(int obj)
        {
            _gamesCount--;

            if (_gamesCount > 0)
            {

                Timer.AddTimer(GoToFightQueue, DateTime.Now);
            }
            else
            {
                Console.WriteLine("Stop " + _client.Login);
            }
        }

        public void Update()
        {
            _client.MainLoopFrame();
            Timer.Instanse.UpdateFrame(DateTime.Now);
        }

        public void Stop()
        {
            _client.Disconnect();
        }
    }
}