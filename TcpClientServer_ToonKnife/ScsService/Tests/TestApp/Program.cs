using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ConnectionIntegrationTest;
using IntegrationTestInfrastructure;

namespace TestApp
{
    class Program
    {
        static List<TestClient> testClient;
        static TestServer testServer;

        static void Main(string[] args)
        {
            Console.WriteLine("Press 1 - server[s], any other - Clietn[s]");
            var input = Console.ReadKey(true).Key;

            TestOptions.current = new ScsServiseTest();
            TestOptions.current.UserCount = 10;

            if (input == ConsoleKey.D1)
            {
                testServer = new TestServer(ScsServiseTest.current.ServerPort);
                Console.Title = "Server[s]";
            }
            else
            {
                testClient = new List<TestClient>();

                for (int i = 0; i < TestOptions.current.UserCount; i++)
                    testClient.Add(new TestClient(TestOptions.current.ServerPort, i));


                Console.Title = "Clirnts[s]";
            }

            Thread loop = new Thread(Loop);
            loop.IsBackground = true;
            loop.Start();

            Console.WriteLine("Press key to exit");
            Console.ReadKey(true);
            loop.Abort();
            Stop();
            Console.WriteLine("Exit");
            Console.ReadKey(true);
        }

        private static void Loop()
        {
            while (true)
            {
                if (testServer != null)
                {
                    testServer.LoopFrame();
                }

                if (testClient != null)
                {
                    foreach (var c in testClient)
                        c.UpdateFrame();
                }
            }
        }

        static void Stop()
        {
            if (testServer != null)
            {
                testServer.Stop();
                testServer.PrintResult();
            }

            if (testClient != null)
            {
                foreach (var c in testClient)
                    c.Disconnect();
            }
        }
    }
}