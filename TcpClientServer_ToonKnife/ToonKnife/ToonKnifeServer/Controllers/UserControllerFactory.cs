using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScsService.Server;
using ToonKnife.Server.DataAsses;
using ToonKnife.Server.Fight;

namespace ToonKnife.Server.Controllers
{
    public class UserControllerFactory : IControllersFactory
    {
        User _user;
        UserFightQueue _userFightQueue;

        public string Login => _user.Login;

        public UserControllerFactory(User user, UserFightQueue userFightQueue)
        {
            _user = user ?? throw new ArgumentNullException(nameof(user));

            _userFightQueue = userFightQueue ?? throw new ArgumentNullException(nameof(userFightQueue));
        }

        public IMainController CreateMainController()
        {
            return new MainUserController(this, _user, _userFightQueue);
        }

        public IFighterController CreateFighterController(Fight.Fight fight, int kifeIndexInFight)
        {
            return new UserFighterController(_user, fight, kifeIndexInFight);
        }
    }
}