using System; 
using System.Threading;
using ToonKnife.TestClient;
using ToonKnife.TestServer;

namespace ToonKnife.TestServerApp
{
    class Program
    {
        static BotTestServer _testServer;
        static TestBotClient _testClients;
         
        static void Main(string[] args)
        {
            int serverPort = 5500;
            int wolfCounut = 1;
            string serverIp = "127.0.0.1";

            if (args.Length > 0)
            {
                serverIp = args[0]; 
            }

           
            Console.WriteLine("Press 1 - server[s] ");
            Console.WriteLine("Press 2 - one clietn");
            Console.WriteLine("Press 3 - server and one serverBot");
            Console.WriteLine("Press any other - Clietn[s]");
            var input = Console.ReadKey(true).Key;
            Console.Clear();

            if (input == ConsoleKey.D1)
            {
                _testServer = new BotTestServer(serverPort, wolfCounut);
                Console.Title = "Server[s]";
                
            }
            else if (input == ConsoleKey.D2)
            {
                _testClients = new TestBotClient(serverPort, serverIp, wolfCounut, 1);
                Console.Title = "Client";
            }
            else if (input == ConsoleKey.D3)
            {
                _testServer = new BotTestServer(serverPort, wolfCounut);
                _testClients = new TestBotClient(serverPort, serverIp, wolfCounut, 1);
                Console.Title = "Server[s] and Client[s]";
            }
            else
            {
                _testClients = new TestBotClient(serverPort, serverIp, wolfCounut, 1);
                Console.Title = "Clients[s]";
            }

            Console.WriteLine("Server ip= " + serverIp);


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
                if (_testServer != null)
                    _testServer.UpdateLoop();

                if (_testClients != null)
                    _testClients.Update();
            }
        }

        static void Stop()
        {
            if (_testServer != null)
                _testServer.Stop();

            if (_testClients != null)
                _testClients.Stop();
        }
    }
}