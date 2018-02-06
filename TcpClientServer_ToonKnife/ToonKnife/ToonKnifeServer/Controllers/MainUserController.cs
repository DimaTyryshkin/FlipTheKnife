using Assets.game.model.knife;
using ScsService.Common;
using ScsService.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToonKnife.Common.ScsMessages;

using ToonKnife.Server.DataAsses;

namespace ToonKnife.Server.Controllers
{
    public class MainUserController
    {
        User _user;
        UserFightQueue _userFightQueue;
        SettingsStorage _knifeDefStorage;

        public MainUserController(User user, UserFightQueue userFightQueue, SettingsStorage knifeDefStorage)
        {
            _knifeDefStorage = knifeDefStorage ?? throw new ArgumentNullException(nameof(knifeDefStorage));
            _user = user ?? throw new ArgumentNullException(nameof(user));
            _userFightQueue = userFightQueue ?? throw new ArgumentNullException(nameof(userFightQueue));


            _user.MsgReaders.RegisterMsgReader<FightQueueMessage>(FightQueueMessage_Reader);
            _user.Disconnected += User_Disconnected;
        }

        private void User_Disconnected(object sender, ClientEvent e)
        {
            throw new NotImplementedException();
        }


        //---msg readrs

        void FightQueueMessage_Reader(ReceivedMsg receivedMsg, FightQueueMessage msg)
        {
            UserFightQueue.Entry entry = new UserFightQueue.Entry(_user, msg.KnifeName, msg.KnifeMode);
            _userFightQueue.AddUser(entry);
        }
    }
}