﻿using System;
using System.Collections.Generic; 

using Hik.Communication.Scs.Communication.EndPoints.Tcp; 
using Hik.Communication.Scs.Server;

using ScsService.Server.Authentication;
using ScsService.Server;
using ToonKnife.Server.DataAsses;
using ToonKnife.Server.Controllers;
using ToonKnife.Server.Fight;
using System.Linq;
using DataAsses.Config;

namespace ToonKnife.Server
{
    public class Server
    {
        ScsService.Server.ScsService _scsServer;
        UserFightQueue _userFightQueue;
        SettingsStorage _setingsStorage;
        FightList _fightList;

        Dictionary<User, IMainController> _userControllers;

        public Server(int tcpPort)
        {
            _userControllers = new Dictionary<User, IMainController>();
            _setingsStorage = new SettingsStorage(new TextFileConfig("ToonKnife"));
            _fightList = new FightList(_setingsStorage);

            _userFightQueue = new UserFightQueue();
            _userFightQueue.UsersReady += UserFightQueue_UserPairReady;

            _scsServer = new ScsService.Server.ScsService(5500);
            _scsServer.OnUserLogin += ScsServer_OnUserLogin;
            _scsServer.OnUserDisconnected += ScsServer_OnUserDisconnected;
        }

        private void ScsServer_OnUserLogin(object sender, UserEventArgs e)
        {
            UserControllerFactory userControllerFactory = new UserControllerFactory(e.User, _userFightQueue);

            IMainController mainUserController = userControllerFactory.CreateMainController();
            _userControllers.Add(e.User, mainUserController);
        }


        private void ScsServer_OnUserDisconnected(object sender, UserEventArgs e)
        {
            _userControllers.Remove(e.User);
        }

        private void UserFightQueue_UserPairReady(object sender, UserFightQueue.Entry[] users)
        {
            var fight = _fightList.CreateAndAddNewFight(users);
            fight.SendEnemyInfo();
        }


        public void Stop()
        {
            _scsServer.Stop();
        }
    }
}