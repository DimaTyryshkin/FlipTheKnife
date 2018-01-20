using System;
using System.Collections.Generic; 

using Hik.Communication.Scs.Communication.EndPoints.Tcp;
using Hik.Communication.Scs.Communication.Messages;
using Hik.Communication.Scs.Server;

namespace ToonKnife.Server
{
    public class Server
    {
        IScsServer server;

        Dictionary<IScsServerClient, ToonKnifeUser> toonKnifeClients;

        public Server(int tcpPort)
        { 
            server = ScsServerFactory.CreateServer(new ScsTcpEndPoint(tcpPort));

            server.ClientConnected += Server_ClientConnected;
            server.ClientDisconnected += Server_ClientDisconnected;

            server.Start();
        }

        public void Stop()
        {
            server.Stop();
        }

        void Server_ClientConnected(object sender, ServerClientEventArgs e)
        {
            Console.WriteLine("A new client is connected. Client Id = " + e.Client.ClientId);

            //Register to MessageReceived event to receive messages from new client
            //e.Client.MessageReceived += Client_MessageReceived;
        }

        void Server_ClientDisconnected(object sender, ServerClientEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}