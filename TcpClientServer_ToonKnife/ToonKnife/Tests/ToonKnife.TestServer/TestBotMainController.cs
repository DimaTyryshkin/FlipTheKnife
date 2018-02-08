using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToonKnife.Common.ScsMessages;
using ToonKnife.Server;
using ToonKnife.Server.Controllers;
using ToonKnife.Server.DataAsses;

namespace ToonKnife.TestServer
{
    public class TestBotMainController : IMainController
    {
        TestBotControllerFactory _testBotControllerFactory;
        UserFightQueue _userFightQueue;

        public string Login { get; private set; }

        public TestBotMainController(TestBotControllerFactory testBotControllerFactory, UserFightQueue userFightQueue, string login)
        {
            _testBotControllerFactory = testBotControllerFactory ?? throw new ArgumentNullException(nameof(testBotControllerFactory));
            _userFightQueue = userFightQueue ?? throw new ArgumentNullException(nameof(userFightQueue));

            Login = login ?? throw new ArgumentNullException(nameof(login));
        }

        public void GoToFightQueue()
        {
            UserFightQueue.Entry entry = new UserFightQueue.Entry(_testBotControllerFactory, "knife00", Assets.game.model.knife.KnifeMode.Medium);
            _userFightQueue.Enqueue(entry);
        }
    }
}