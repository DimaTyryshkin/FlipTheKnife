using Assets.game.model.knife;
using ScsService.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToonKnife.Server
{
    /// <summary>
    /// Очередь клдиентов на бой
    /// </summary>
    public class UserFightQueue
    {
        public class Entry
        {
            public User user;
            public string knifeName;
            public KnifeMode knifeMode;
            public DateTime timeEnqueue;

            public Entry(User user, string knifeName, KnifeMode knifeMode )
            {
                this.user = user ?? throw new ArgumentNullException(nameof(user));
                this.knifeName = knifeName ?? throw new ArgumentNullException(nameof(knifeName));
                this.knifeMode = knifeMode; 
            }
        }



        Entry _awaitingUser;


        public event EventHandler<Entry[]> UsersReady;

        public void AddUser(Entry user)
        {
            user.timeEnqueue = DateTime.Now;

            if (_awaitingUser == null)
            {
                _awaitingUser = user;
            }
            else
            {
                var oldUser = _awaitingUser;
                _awaitingUser = null;

                UsersReady?.Invoke(this, new Entry[] { oldUser, user });
            }
        }

        public void RemoveUser(Entry user)
        {
            if (_awaitingUser == user)
            {
                _awaitingUser = null;
            }
        }
    }
}