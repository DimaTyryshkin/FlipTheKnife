using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Text;
using System.Linq;

namespace ThreadTest
{
    class Program
    {
        struct St : IEquatable<St>
        {
            public int d1;
            public int d2;
            public int d3;

            public override bool Equals(object obj)
            {
                return base.Equals(obj);
            }

            public bool Equals(St a)
            {
                return this == a;
            }

            public static bool operator ==(St a, St b)
            {
                return a.d1 == b.d1 && a.d1 == b.d2 && a.d3 == b.d3;
            }

            public static bool operator !=(St a, St b)
            {
                return !(a == b);
            }

            public override string ToString()
            {
                return d1.ToString() + " " + d2.ToString() + " " + d3.ToString();
            }
        }

        static St f;
        static St f1 = new St
        {
            d1 = 4444444,
            d2 = 4444444,
            d3 = 4444444
        };

        static St f2 = new St
        {
            d1 = 55555,
            d2 = 55555,
            d3 = 55555
        };

        static void Main(string[] args)
        {
            Thread t1 = new Thread(Loop1);
            Thread t2 = new Thread(Loop2);

            t1.IsBackground = true;
            t2.IsBackground = true;

            t1.Start();
            t2.Start();
             
            Console.ReadKey(true);
        }


        static void Loop1()
        {
            while (true)
            {
                f = f1;

                var fTemp = f;
                Check(fTemp);
            }

        }

        static void Loop2()
        {
            while (true)
            {
                f = f2;

                var fTemp = f;
                Check(fTemp);
            }
        }

        static void Check(St fTemp)
        {
            if (fTemp != f1 && fTemp != f2)
            {
                Console.WriteLine(fTemp);
            }
        }
    }
}