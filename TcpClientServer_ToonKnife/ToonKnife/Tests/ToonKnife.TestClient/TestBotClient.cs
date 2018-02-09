using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToonKnife.TestServer;

namespace ToonKnife.TestClient
{
    public class TestBotClient
    {
        List<BotClient> _listClients;

        public TestBotClient(int serverPort, string serverIp, int wolfsCount, int fightsPerUser)
        {
            for (int i = 0; i < wolfsCount; i++)
            {
                {
                    Console.WriteLine(typeof(TestBotClient).Name + $"Create boots i={i}");
                    BotClient wolf = new BotClient(serverPort, serverIp, "TestBot_Wolf_" + i, true, fightsPerUser);
                    _listClients.Add(wolf);
                }

                {
                    Console.WriteLine(typeof(TestBotClient).Name + $"Create boots i={i}");
                    BotClient wolf = new BotClient(serverPort, serverIp, "TestBot_Rabbit_" + i, false, fightsPerUser);
                    _listClients.Add(wolf);
                }
            }
        }

        public void Update()
        {
            foreach (var c in _listClients)
                c.Update();

        }

        public void Stop()
        {
            foreach (var c in _listClients)
                c.Stop();
        }
    }
}