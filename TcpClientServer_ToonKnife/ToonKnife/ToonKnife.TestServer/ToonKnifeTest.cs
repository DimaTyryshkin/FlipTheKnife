using IntegrationTestInfrastructure;
using ScsService.Common;
using ScsService.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToonKnife.TestServer
{
    public class ToonKnifeTest : TestOptions
    {
        public Step Step_AllUsers_EnqueueInFightQueue = new Step()
        {
            name = "Дождаться пока в очереде на бой побывают все юзеры(каждый встал в очередь)"
        };

        public Step Step_AllUsers_WaitForFight_And_SeayReadyToFight = new Step()
        {
            name = "Все юзеры дождались начала боя и прислалаи подтверждение готовнсоти"

        };

        public Step Step_AllRabbitUsers_SendFail = new Step()
        {
            name = "Всем ботам-Rabbit отправленно сообщение что они проигарли"
        };

        public Step Step_AllWolfUsers_SendWin = new Step()
        {
            name = "Всем ботам-wolf отправленно сообщение что они выйграли"
        };
    }
}