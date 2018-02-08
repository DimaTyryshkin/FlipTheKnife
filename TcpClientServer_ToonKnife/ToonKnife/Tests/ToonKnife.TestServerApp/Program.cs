using ConnectionIntegrationTest;
using IntegrationTestInfrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ToonKnife.TestServer;

namespace ToonKnife.TestServerApp
{
    class Program
    {
        static BotTestServer testServer;
        static void Main(string[] args)
        {
            testServer = new BotTestServer();


            Thread loop = new Thread(Loop);
            loop.IsBackground = true;
            loop.Start();

            Console.WriteLine("Press key to exit");
            Console.ReadKey(true);
            loop.Abort();

            testServer.Stop();
            Console.WriteLine("Exit");
            Console.ReadKey(true);
        }

        private static void Loop()
        {
            while (true)
            {
                testServer.UpdateLoop();
            }
        }
    }
}