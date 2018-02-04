using System;
using ScsService.Client;
using System.Collections.Generic;
using System.Linq;

namespace ConnectionIntegrationTest
{
    public class TestClientEntryPoint
    {
        List<TestClient> clietns;
        DateTime timeToDisconnect;
        bool isRun = true;

        public TestClientEntryPoint(int serverPort)
        {
            clietns = new List<ScsClient>();

            foreach (var index in Enumerable.Range(0, TestOptions.UserCount))
            {
                var c = new TestClient(serverPort, index);
                clietns.Add(c);

                c.clietn.OnUserLogin += C_OnUserLogin;
            }
        }

        private void C_OnUserLogin(object sender, EventArgs e)
        {
            //((ScsClient)sender).Disconnect();
        }

        public void UpdateFrame()
        {
            if (isRun)
            {
                foreach (var clietn in clietns)
                    clietn.MainLoopFrame();
            }
        }

        public void Disconnect()
        {
            isRun = false;

            foreach (var clietn in clietns)
            {
                clietn.Disconnect();
            }
        }
    }
}
