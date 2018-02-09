using System;
using System.Collections.Generic;
using System.Linq;
using ScsService.Server;
using ToonKnife.Server.Controllers;
using ToonKnife.Server.Fight;

namespace ToonKnife.Server.ScsServiceAdapter.Controllers
{
    public class UserControllerFactory : IControllersFactory
    {
        User _user; 

        public string Login => _user.Login;

        public UserControllerFactory(User user)
        {
            _user = user ?? throw new ArgumentNullException(nameof(user));

            
        }

        public IMainController CreateMainController(UserFightQueue userFightQueue)
        {
            return new MainUserController(this, _user, userFightQueue);
        }

        public IFighterController CreateFighterController(Fight.Fight fight, int kifeIndexInFight)
        {
            return new UserFighterController(_user, fight, kifeIndexInFight);
        }
    }
}