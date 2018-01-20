using Hik.Communication.Scs.Communication.Messengers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScsService.Server.Authentication
{
    public struct User
    {
        public IMessenger Messenger { get; private set; }
        public string Login { get; private set; }

        public User(string login, IMessenger messenger)
        {
            Messenger = messenger;
            Login = login;
        }
    }
}