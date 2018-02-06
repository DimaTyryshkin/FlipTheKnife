using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToonKnife.Server.Fight
{
    public class KnifeThrowEventArgs : EventArgs
    {
        public int knifeIndex;
        public DateTime timeThrow;
        public DateTime timeNextThrow;
        public float input;

        public KnifeThrowEventArgs(int knifeIndex, DateTime timeThrow, DateTime timeNextThrow, float input)
        {
            this.knifeIndex = knifeIndex;
            this.timeThrow = timeThrow;
            this.timeNextThrow = timeNextThrow;
            this.input = input;
        }
    }
}