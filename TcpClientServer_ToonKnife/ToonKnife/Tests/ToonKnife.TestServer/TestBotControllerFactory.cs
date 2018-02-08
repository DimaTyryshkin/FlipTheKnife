using System;
using ToonKnife.Server;
using ToonKnife.Server.Controllers;
using ToonKnife.Server.DataAsses;
using ToonKnife.Server.Fight;

namespace ToonKnife.TestServer
{
    public class TestBotControllerFactory : IControllersFactory
    {
        string _login;
        UserFightQueue _userFightQueue;
        SettingsStorage _knifeDefStorage;
        bool _isWoll;
        ToonKnifeTest _test;

        public string Login => _login;


        public TestBotControllerFactory(string login, UserFightQueue userFightQueue, SettingsStorage knifeDefStorage, bool isWoll, ToonKnifeTest test)
        {
            _knifeDefStorage = knifeDefStorage ?? throw new ArgumentNullException(nameof(knifeDefStorage));
            _isWoll = isWoll;
            _test = test;
            _login = login ?? throw new ArgumentNullException(nameof(login));
            _userFightQueue = userFightQueue ?? throw new ArgumentNullException(nameof(userFightQueue));
        }



        public IFighterController CreateFighterController(Fight fight, int kifeIndexInFight)
        {
            return new TestBotFighterController(_login, fight, kifeIndexInFight, _test, _isWoll);
        }

        public IMainController CreateMainController()
        {
            return new TestBotMainController(this, _userFightQueue, _login);
        }
    }
}