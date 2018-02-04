using System;
using System.Collections.Generic; 

using Hik.Communication.Scs.Communication.EndPoints.Tcp; 
using Hik.Communication.Scs.Server;

using ScsService.Server.Authentication;
using ScsService.Server;

namespace ToonKnife.Server
{
    public class Server
    {
        ScsService.Server.ScsService scsServer;
          
        public Server(int tcpPort)
        {
            scsServer = new ScsService.Server.ScsService(5500);
            scsServer.OnUserLogin += ScsServer_OnUserLogin;
            scsServer.OnUserDisconnected += ScsServer_OnUserDisconnected;
        }

        private void ScsServer_OnUserDisconnected(object sender, UserEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ScsServer_OnUserLogin(object sender, UserEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            scsServer.Stop();
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