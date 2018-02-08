using Assets.game.model.knife;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToonKnife.Server.Controllers;

namespace ToonKnife.Server.Fight
{
    public interface IFighterController : INamedController
    {
        void SendFightCreated(string enemyName, string enemyKnifeName, KnifeMode enemyKnifeMode);
    }
}