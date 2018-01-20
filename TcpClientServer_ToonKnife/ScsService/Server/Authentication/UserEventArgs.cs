using System;

namespace ScsService.Server.Authentication
{
    public class UserEventArgs : EventArgs
    {
        public User User { get; private set; }

        public UserEventArgs(User user)
        {
            User = user;
        }
    }
}