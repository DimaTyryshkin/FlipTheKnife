using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToonKnife.Server;
using ToonKnife.TestServer;

namespace ToonKnife.TestClient
{
    public class TestBotClient
    {
        List<BotClient> _listClients;
        int _serverPort;
        string _serverIp;
        int _wolfsCount;
        private readonly int _fightsPerUser;

        public TestBotClient(int serverPort, string serverIp, int wolfsCount, int fightsPerUser)
        {
            _listClients = new List<BotClient>();

            _serverPort = serverPort;
            _serverIp = serverIp;
            _wolfsCount = wolfsCount;
            _fightsPerUser = fightsPerUser;

            AddPair();
        }

        void AddPair()
        {
            int i = _wolfsCount - 1;
            _wolfsCount--;

            {
                Console.WriteLine(typeof(TestBotClient).Name + $"Create bot i={i}");
                BotClient wolf = new BotClient(_serverPort, _serverIp, "TestBot_Wolf_" + i, true, _fightsPerUser);
                _listClients.Add(wolf);
            }

            //{
            //    Console.WriteLine(typeof(TestBotClient).Name + $"Create bot i={i}");
            //    BotClient wolf = new BotClient(_serverPort, _serverIp, "TestBot_Rabbit_" + i, false, _fightsPerUser);
            //    _listClients.Add(wolf);
            //}

            if (_wolfsCount > 0)
            {
                Timer.AddTimer(AddPair, DateTime.Now + TimeSpan.FromSeconds(1f));
            }
        }

        public void Update()
        {
            foreach (var c in _listClients.ToArray())
                c.Update();

        }

        public void Stop()
        {
            foreach (var c in _listClients.ToArray())
                c.Stop();
        }
    }
}