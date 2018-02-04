using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectionIntegrationTest
{
    public enum TestStep
    {
        NoStep = 0,
        StepConnect_And_Autentificate_AllClient,
        Step_SendMsg_And_GetResponse_FromAllClients_UsingConcurrentEventQueue_And_MsgReadersCollection,

        StepDisconnectAllClient
    }

    public class TestStepProgres
    {
        TestStep _step;

        public TestStep Step
        {
            get { return _step; }
            set
            {
                if ((int)Step == (int)value - 1)
                {
                    _step = value;
                }
                else
                {
                    Errors++;
                }
            }
        }

        public int Errors { get; private set; }

    }
}