using Assets.game.model.knife;
using ClientServerGameLogicCommon;
using ScsService.Client;
using ScsService.Common;
using System; 
using ToonKnife.Common.ScsMessages;

namespace ToonKnife.Client.ScsServiceAdapter
{
    public class MainServerController
    {
        ScsClient _client;


        public event Action<MainServerController, FightController, EnemyDef> FigthCreated;


        public MainServerController(ScsClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));

            _client.MsgReaders.RegisterMsgReader<FightCteatedMessage>(FightCteated_Reader);
        }


        public void GoToFightQueue(string knifeName, KnifeMode knifeMode)
        {
            FightQueueMessage fightQueueMessage = new FightQueueMessage(knifeName, knifeMode);
            _client.Messenger.SendMessage(fightQueueMessage);
        }

        //---readers

        void FightCteated_Reader(ReceivedMsg receivedMsg, FightCteatedMessage msg)
        {
            FightController fightController = new FightController(_client, msg.KnifeIndex);
            EnemyDef enemyDef = new EnemyDef(msg.EnemyUserName, msg.EnemyKnifeName, msg.EnemyKnifeMode);
            FigthCreated?.Invoke(this, fightController, enemyDef);
        }
    }
}