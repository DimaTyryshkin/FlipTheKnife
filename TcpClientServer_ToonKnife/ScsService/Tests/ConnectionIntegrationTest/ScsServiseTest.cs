using IntegrationTestInfrastructure;
using ScsService.Common;
using ScsService.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectionIntegrationTest
{
    public class ScsServiseTest : TestOptions
    {
        public static Step StepConnect_And_Autentificate_AllClient = new Step()
        {
            name = "Приянть на сервере подключения от всех клиентов и дождатся пока они все пройдут аунтентификацию"
        };

        public static Step Step_SendMsg_And_GetResponse_FromAllClients_UsingConcurrentEventQueue_And_MsgReadersCollection = new Step()
        {
            name = "послать сообщение всем клиентам и получить от всех ответы, " +
            "с использованием " + nameof(MsgReadersCollection)
        };

        public static Step StepDisconnectAllClient = new Step()
        {
            name = "Дождаться отключения всех клиентов"
        };
    }
}