using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToonKnife.Common
{
    public class KnifeThrowEventArg : EventArgs
    {
        public int KnifeId { get; }

        public float Input { get; }
        public DateTime TimeNextThrow { get; }
        public DateTime TimeThrow { get; }

        public KnifeThrowEventArg(int knifeId, float input, DateTime timeNextThrow, DateTime timeThrow)
        {
            KnifeId = knifeId;
            Input = input;
            TimeNextThrow = timeNextThrow;
            TimeThrow = timeThrow;
        }
    }
}