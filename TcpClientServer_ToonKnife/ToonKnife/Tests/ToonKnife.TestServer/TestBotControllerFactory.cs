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
        bool _isWoll;
        ToonKnifeTest _test;

        public string Login => _login;


        public TestBotControllerFactory(string login, bool isWoll, ToonKnifeTest test)
        {
            _isWoll = isWoll;
            _test = test;
            _login = login ?? throw new ArgumentNullException(nameof(login));
        }

        public IFighterController CreateFighterController(Fight fight, int kifeIndexInFight)
        {
            return new TestBotFighterController(_login, fight, kifeIndexInFight, _test, _isWoll);
        }

        public IMainController CreateMainController(UserFightQueue userFightQueue)
        {
            var m = new TestBotMainController(this, userFightQueue, _login);
            m.GoToFightQueue();//бот сразу идет в очередь

            return m;
        }
    }
}