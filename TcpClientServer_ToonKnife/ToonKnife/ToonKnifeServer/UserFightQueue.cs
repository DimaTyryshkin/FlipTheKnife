using Assets.game.model.knife;
using ScsService.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToonKnife.Server.Controllers;

namespace ToonKnife.Server
{
    /// <summary>
    /// Очередь клдиентов на бой
    /// </summary>
    public class UserFightQueue
    {
        public class Entry
        {
            public IControllersFactory controllersFactory;
            public string knifeName;
            public KnifeMode knifeMode;
            public DateTime timeEnqueue;

            public Entry(IControllersFactory controllersFactory, string knifeName, KnifeMode knifeMode)
            {
                this.controllersFactory = controllersFactory ?? throw new ArgumentNullException(nameof(controllersFactory));
                this.knifeName = knifeName ?? throw new ArgumentNullException(nameof(knifeName));
                this.knifeMode = knifeMode;
            }
        }



        Entry _awaitingUser;

        public event Action<UserFightQueue, Entry> UserEnqueue;
        public event Action<UserFightQueue, Entry[]> UsersReady;

        public void Enqueue(Entry entry)
        {
            entry.timeEnqueue = DateTime.Now;

            UserEnqueue?.Invoke(this, entry);

            if (_awaitingUser == null)
            {
                _awaitingUser = entry;
            }
            else
            {
                var oldUser = _awaitingUser;
                _awaitingUser = null;

                UsersReady?.Invoke(this, new Entry[] { oldUser, entry });
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