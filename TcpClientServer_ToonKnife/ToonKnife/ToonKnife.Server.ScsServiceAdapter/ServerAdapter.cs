
using ScsService.Server;
using ScsService.Server.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ToonKnife.Server.ScsServiceAdapter.Controllers;

namespace ToonKnife.Server.ScsServiceAdapter
{
    public class ServerAdapter
    {
        ScsService.Server.ScsService _scsServer;
        Server _toonKnifeServer;

        public ServerAdapter(int serverPort)
        {
            _toonKnifeServer = new Server();

            _scsServer = new ScsService.Server.ScsService(5500);
            _scsServer.OnUserLogin += ScsServer_OnUserLogin;
            _scsServer.OnUserDisconnected += ScsServer_OnUserDisconnected;
        }

        public void Update()
        {
            _scsServer.MainLoopFrame();
            _toonKnifeServer.UpdateLoop();
        }

        public void Stop()
        {
            _scsServer.Stop();
            _toonKnifeServer.Stop();
        }


        void ScsServer_OnUserLogin(object sender, UserEventArgs e)
        {
            UserControllerFactory userControllerFactory = new UserControllerFactory(e.User);
            _toonKnifeServer.AddUser(userControllerFactory);
        }

        void ScsServer_OnUserDisconnected(object sender, UserEventArgs e)
        {
            _toonKnifeServer.RemoveUser(e.User.Login);
        }
    }
}