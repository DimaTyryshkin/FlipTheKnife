using System;
using System.Linq;
using System.Collections.Generic; 

using ToonKnife.Server.DataAsses;
using ToonKnife.Server.Controllers;
using ToonKnife.Server.Fight;
using DataAsses.Config;

namespace ToonKnife.Server
{
    public class Server
    { 
        UserFightQueue _userFightQueue;
        SettingsStorage _setingsStorage;
        FightList _fightList;

        List<IMainController> _userMainController;
        Dictionary<string, IControllersFactory> _allUsers;

        //---prop
        public FightList FightList => _fightList;

        public UserFightQueue UserFightQueue => _userFightQueue;


        //---methods
        public Server()
        {
            _allUsers = new Dictionary<string, IControllersFactory>();
            _userMainController = new List<IMainController>();

            _setingsStorage = new SettingsStorage(new TextFileConfig("ToonKnife"));
            _fightList = new FightList(_setingsStorage);

            _userFightQueue = new UserFightQueue();
            _userFightQueue.UsersReady += UserFightQueue_UserPairReady;

            //_scsServer = new ScsService.Server.ScsService(5500);
            //_scsServer.OnUserLogin += ScsServer_OnUserLogin;
            //_scsServer.OnUserDisconnected += ScsServer_OnUserDisconnected;
        }

        public void UpdateLoop()
        {
            Timer.Instanse.UpdateFrame(DateTime.Now);
        }

        public void Stop()
        { 
        }


        public void AddUser(IControllersFactory controllersFactory)
        {
            _allUsers.Add(controllersFactory.Login, controllersFactory);

            IMainController mainUserController = controllersFactory.CreateMainController(_userFightQueue);
            _userMainController.Add(mainUserController);
        }

        public void RemoveUser(string login)
        {
            _allUsers.Remove(login);
            _userMainController.RemoveAll(c => c.Login == login);
        }


        void UserFightQueue_UserPairReady(object sender, UserFightQueue.Entry[] users)
        {
            var fight = _fightList.CreateAndAddNewFight(users);
            fight.SendEnemyInfo();
        }
    }
}