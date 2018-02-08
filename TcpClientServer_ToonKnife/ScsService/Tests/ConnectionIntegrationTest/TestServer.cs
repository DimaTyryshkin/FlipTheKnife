 
using ScsService.Server.Authentication;
using ScsService.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using ScsService.Common;
using ConnectionIntegrationTest.ScsMessages;
using Hik.Communication.Scs.Communication.Messengers;

namespace ConnectionIntegrationTest
{
    public class TestServer
    {
        ScsService.Server.ScsService scsServer;

        Dictionary<IMessenger, User> _users;

        public TestServer(int tcpPort)
        {
            _users = new Dictionary<IMessenger, User>();

            scsServer = new ScsService.Server.ScsService(ScsServiseTest.current.ServerPort);
            scsServer.MsgReaders.RegisterMsgReader<TestMessage>(OnTestMessage);

            scsServer.OnUserLogin += ScsServer_OnUserLogin;
            scsServer.OnUserDisconnected += ScsServer_OnUserDisconnected;

            scsServer.Start();
        }

        public void LoopFrame()
        {
            scsServer.MainLoopFrame();
        }

        public void Stop()
        {
            scsServer.Stop();
        }

        public void PrintResult()
        {
            ScsServiseTest.current.PrintAllResult();
        }


        //---connect disconnect
        void ScsServer_OnUserLogin(object sender, UserEventArgs e)
        {
            _users.Add(e.User.Client, e.User);

            Console.WriteLine("A new User is connected. User.Login =" + e.User.Login);

            ScsServiseTest.StepConnect_And_Autentificate_AllClient.IncrimentCounter(e.User.Login);

            e.User.Client.SendMessage(new TestMessage(TestMessage.State.Step_SendMsg_And_GetResponse_FromAllClients_UsingConcurrentEventQueue_And_MsgReadersCollection_1));
        }

        void ScsServer_OnUserDisconnected(object sender, UserEventArgs e)
        {
            _users.Remove(e.User.Client);

            Console.WriteLine("User is Disconnected. User.Login =" + e.User.Login);

            ScsServiseTest.StepDisconnectAllClient.IncrimentCounter(e.User.Login);
        }


        //---Users
        User GetUser(ReceivedMsg msg)
        {
            return _users[msg.Sender];
        }

        int GetUserIndex(ReceivedMsg msg)
        {
            return ScsServiseTest.GetUserIndexFromLogin(GetUser(msg).Login);
        }


        //---msg readers
        void OnTestMessage(ReceivedMsg receivedMsg, TestMessage msg)
        {
            if (msg._state == TestMessage.State.Step_SendMsg_And_GetResponse_FromAllClients_UsingConcurrentEventQueue_And_MsgReadersCollection_2)
            {
                ScsServiseTest.Step_SendMsg_And_GetResponse_FromAllClients_UsingConcurrentEventQueue_And_MsgReadersCollection.IncrimentCounter(GetUser(receivedMsg).Login);
            }
        }
    }
}