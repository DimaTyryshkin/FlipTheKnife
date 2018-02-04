using ScsService.Common;
using ScsService.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectionIntegrationTest
{
    public static class TestOptions
    {
        public struct Step
        {
            public string name;
            public bool succes;
             
            public int currentValue;

            public void SetSuccesAntPrintState()
            {
                succes = true;
                PrintState();
            }

            public void PrintState()
            {
                Console.WriteLine((succes ? stepSucces : stepFail)+ $" : ({currentValue}) {name}");
            }
        }


        public static TimeSpan testMaxTime = TimeSpan.FromSeconds(10);
        public static int ServerPort = 5500;
        public static int UserCount =50;
        


        //--- строковые константы
        static string userNamePrefix = "User_";

        static string stepSucces = "выполнен";
        static string stepFail = "провален";
        static string testTimeEnd = "Тест не успел выполниться (провален)";

        #region Шаги теста

        public static bool IsAllStepsSucces()
        {
            var steps = GetAllSteps();

            foreach (var step in steps.Where(s => !s.succes))
                step.PrintState();

            return steps.All(s => s.succes);
        }

        static Step[] GetAllSteps()
        {
            var allFields = typeof(TestOptions).GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);

            return allFields
                .Where(f => f.FieldType == typeof(Step))
                .Select(f => f.GetValue(null))
                .Cast<Step>()
                .ToArray();
        }

        public static void PrintAllResult()
        {
            Console.WriteLine($"UserCount = {UserCount}");

            var steps = GetAllSteps();

            foreach (var step in steps)
                step.PrintState();
        }

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

        #endregion



        public static string GetUserLogin(int userIndex)
        {
            return userNamePrefix + userIndex;
        }

        public static int GetUserIndexFromLogin(string userName)
        {
            string index = userName.Split('_')[1];
            return int.Parse(index);
        }
    }
}