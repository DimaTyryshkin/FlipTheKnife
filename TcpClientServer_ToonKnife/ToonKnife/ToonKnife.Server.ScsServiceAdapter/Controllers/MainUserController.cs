using ScsService.Common;
using ScsService.Server;
using System;
using ToonKnife.Common.ScsMessages;
using ToonKnife.Server.Controllers; 

namespace ToonKnife.Server.ScsServiceAdapter.Controllers
{
    public class MainUserController : IMainController
    {
        UserControllerFactory _controllersFactory;
        User _user;
        UserFightQueue _userFightQueue; 

        public string Login => _user.Login;

        public MainUserController(UserControllerFactory controllersFactory, User user, UserFightQueue userFightQueue)
        { 
            _controllersFactory = controllersFactory ?? throw new ArgumentNullException(nameof(controllersFactory));
            _user = user ?? throw new ArgumentNullException(nameof(user));
            _userFightQueue = userFightQueue ?? throw new ArgumentNullException(nameof(userFightQueue));


            _user.MsgReaders.RegisterMsgReader<FightQueueMessage>(FightQueueMessage_Reader);
            _user.Disconnected += User_Disconnected;
        }

        void User_Disconnected(object sender, ClientEvent e)
        {
            throw new NotImplementedException();
        }


        //---msg readrs

        void FightQueueMessage_Reader(ReceivedMsg receivedMsg, FightQueueMessage msg)
        {
            // TODO проверку чтобы 2 раза в очередь нельзя было вставать
            UserFightQueue.Entry entry = new UserFightQueue.Entry(_controllersFactory, msg.KnifeName, msg.KnifeMode);
            _userFightQueue.Enqueue(entry);
        }
    }
}