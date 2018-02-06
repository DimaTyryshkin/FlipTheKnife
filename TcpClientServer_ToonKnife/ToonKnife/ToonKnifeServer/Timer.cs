using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToonKnife.Server
{
    public class Timer
    {
        static Timer _instanse;
        public static Timer Instanse
        {
            get
            {
                if (_instanse == null)
                {
                    _instanse = new Timer();
                }

                return _instanse;
            }

            set
            {
                _instanse = value;
            }
        }

        public static void AddTimer(Action action, DateTime time)
        {
            Instanse.Add(action, time);
        }
         
        struct Entry
        {
            public Action action;
            public DateTime time;

            public Entry(Action action, DateTime time)
            {
                this.action = action ?? throw new ArgumentNullException(nameof(action));
                this.time = time;
            }
        }

        LinkedList<Entry> callList;



        public Timer()
        {
            callList = new LinkedList<Entry>();
        }

        public void Add(Action action, DateTime time)
        {
            callList.AddLast(new Entry(action, time));
        }

        public void UpdateFrame(DateTime now)
        {
            var cur = callList.First;

            while (cur != null)
            {
                if (now >= cur.Value.time)
                {
                    var toRemove = cur;
                    callList.Remove(toRemove);

                    toRemove.Value.action();
                }

                cur = cur.Next;
            }
        }
    }
}