 
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
        List<TestStepProgres> _clietnsTestProgress;


        public TestServer(int tcpPort)
        {
            _users = new Dictionary<IMessenger, User>();

            _clietnsTestProgress = new List<TestStepProgres>();

            foreach (var clietnIndex in Enumerable.Range(0, TestOptions.UserCount))
                _clietnsTestProgress.Add(new TestStepProgres());


            scsServer = new ScsService.Server.ScsService(TestOptions.ServerPort);
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
            Console.WriteLine("-------------------------------------");
            if (_clietnsTestProgress.All(p => (int)p.Step >= (int)TestStep.StepConnect_And_Autentificate_AllClient))
                TestOptions.StepConnect_And_Autentificate_AllClient.succes = true;

            if (_clietnsTestProgress.All(p => (int)p.Step >= (int)TestStep.Step_SendMsg_And_GetResponse_FromAllClients_UsingConcurrentEventQueue_And_MsgReadersCollection))
                TestOptions.Step_SendMsg_And_GetResponse_FromAllClients_UsingConcurrentEventQueue_And_MsgReadersCollection.succes = true;

            if (_clietnsTestProgress.All(p => (int)p.Step >= (int)TestStep.StepDisconnectAllClient))
                TestOptions.StepDisconnectAllClient.succes = true;

            TestOptions.PrintAllResult();
        }


        //---connect disconnect
        void ScsServer_OnUserLogin(object sender, UserEventArgs e)
        {
            _users.Add(e.User.Client, e.User);

            Console.WriteLine("A new User is connected. User.Login =" + e.User.Login);

            _clietnsTestProgress[TestOptions.GetUserIndexFromLogin(e.User.Login)].Step = TestStep.StepConnect_And_Autentificate_AllClient;

            TestOptions.StepConnect_And_Autentificate_AllClient.currentValue++;
            e.User.Client.SendMessage(new TestMessage(TestMessage.State.Step_SendMsg_And_GetResponse_FromAllClients_UsingConcurrentEventQueue_And_MsgReadersCollection_1));
        }

        void ScsServer_OnUserDisconnected(object sender, UserEventArgs e)
        {
            _users.Remove(e.User.Client);

            Console.WriteLine("User is Disconnected. User.Login =" + e.User.Login);

            TestOptions.StepDisconnectAllClient.currentValue++;
            _clietnsTestProgress[TestOptions.GetUserIndexFromLogin(e.User.Login)].Step = TestStep.StepDisconnectAllClient;
        }


        //---Users
        User GetUser(ReceivedMsg msg)
        {
            return _users[msg.Sender];
        }

        int GetUserIndex(ReceivedMsg msg)
        {
            return TestOptions.GetUserIndexFromLogin(GetUser(msg).Login);
        }


        //---msg readers
        void OnTestMessage(ReceivedMsg receivedMsg, TestMessage msg)
        {
            if (msg._state == TestMessage.State.Step_SendMsg_And_GetResponse_FromAllClients_UsingConcurrentEventQueue_And_MsgReadersCollection_2)
            {
                TestOptions.Step_SendMsg_And_GetResponse_FromAllClients_UsingConcurrentEventQueue_And_MsgReadersCollection.currentValue++;
             _clietnsTestProgress[GetUserIndex(receivedMsg)].Step = TestStep.Step_SendMsg_And_GetResponse_FromAllClients_UsingConcurrentEventQueue_And_MsgReadersCollection;
            }
        }
    }
}