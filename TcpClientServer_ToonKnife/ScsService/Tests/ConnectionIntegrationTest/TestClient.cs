
using System;
using ConnectionIntegrationTest.ScsMessages;
using ScsService.Client;
using ScsService.Common;


namespace ConnectionIntegrationTest
{
    public class TestClient
    {
        public ScsClient _clietn;
        public int _index;
         
        public TestClient(int serverPort, int clientIndex)
        {
            _index = clientIndex;
            _clietn = new ScsClient(ScsServiseTest.GetUserLogin(clientIndex), "127.0.0.1", serverPort);
            
            _clietn.OnUserLogin += Client_OnUserLogin;

            _clietn.Connect();
        }

        private void Client_OnUserLogin()
        {
            _clietn.MsgReaders.RegisterMsgReader<TestMessage>(TestMetgod);
            //_clietn.AddMessenger(_clietn.Messenger);
        }

        private void TestMetgod(ReceivedMsg receivedMsg, TestMessage msg)
        {
            Console.WriteLine("TestMetgod"); 

            if (msg._state == TestMessage.State.Step_SendMsg_And_GetResponse_FromAllClients_UsingConcurrentEventQueue_And_MsgReadersCollection_1)
            {
                receivedMsg.Sender.SendMessage(new TestMessage(
                    TestMessage.State.Step_SendMsg_And_GetResponse_FromAllClients_UsingConcurrentEventQueue_And_MsgReadersCollection_2
                    , "test msg"
                    ));
            }

            Disconnect();
        }

        public void UpdateFrame()
        {
            _clietn.MainLoopFrame();
        }

        public void Disconnect()
        {
            _clietn.Disconnect();
        }
    }
}