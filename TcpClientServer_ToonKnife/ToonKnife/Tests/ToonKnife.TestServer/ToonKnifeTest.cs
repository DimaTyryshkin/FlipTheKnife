using IntegrationTestInfrastructure;  

namespace ToonKnife.TestServer
{
    public class ToonKnifeTest : TestOptions
    {
        public int WolfTotalCount => (int)(UserCount/2f);
        public int RabbitTotalCount => WolfTotalCount;

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