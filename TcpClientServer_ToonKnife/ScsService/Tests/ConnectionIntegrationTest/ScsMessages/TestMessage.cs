using Hik.Communication.Scs.Communication.Messages;
using System;

namespace ConnectionIntegrationTest.ScsMessages
{
    [Serializable]
    public class TestMessage : ScsMessage
    {
        public enum State
        {
            NoState = 0,

            Step_SendMsg_And_GetResponse_FromAllClients_UsingConcurrentEventQueue_And_MsgReadersCollection_1 = 1,
            Step_SendMsg_And_GetResponse_FromAllClients_UsingConcurrentEventQueue_And_MsgReadersCollection_2 = 2,
        }

        public State _state;
        public string _additinalInfo;

        public TestMessage(State state, string additinalInfo = null, string repliedMessageId = null) : base(repliedMessageId)
        {
            _state = state;
            _additinalInfo = additinalInfo;
        }
    }
}