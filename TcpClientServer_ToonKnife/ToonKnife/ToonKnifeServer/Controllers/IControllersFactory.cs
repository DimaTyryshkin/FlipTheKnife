using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToonKnife.Server.Fight;

namespace ToonKnife.Server.Controllers
{
    /// <summary>
    /// Фабрика для создания контролеров. Главная реализация- возвращает контроллеры юзера, еще можно возвращать ботов или тестовые котнроллеры.
    /// </summary>
    public interface IControllersFactory
    {
        string Login { get; }

        IMainController CreateMainController();

        IFighterController CreateFighterController(Fight.Fight fight, int kifeIndexInFight);
    }
}