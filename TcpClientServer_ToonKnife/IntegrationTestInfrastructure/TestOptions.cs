using System; 
using System.Linq;

namespace IntegrationTestInfrastructure
{
    public class TestOptions
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
                Console.WriteLine((succes ? stepSucces : stepFail) + $" : ({currentValue}) {name}");
            }
        }

        public static TestOptions current;

        public static string GetUserLogin(int userIndex)
        {
            return userNamePrefix + userIndex;
        }

        public static int GetUserIndexFromLogin(string userName)
        {
            string index = userName.Split('_')[1];
            return int.Parse(index);
        }


        //---dynamic

        public TimeSpan testMaxTime = TimeSpan.FromSeconds(10);
        public int ServerPort = 5500;
        public int UserCount;



        //--- строковые константы
        protected static string userNamePrefix = "User_";

        protected static string stepSucces = "выполнен";
        protected static string stepFail = "провален";
        protected static string testTimeEnd = "Тест не успел выполниться (провален)";



        public bool IsAllStepsSucces()
        {
            var steps = GetAllSteps();

            foreach (var step in steps.Where(s => !s.succes))
                step.PrintState();

            return steps.All(s => s.succes);
        }

        public Step[] GetAllSteps()
        {
            var allFields = GetType().GetFields(System.Reflection.BindingFlags.Instance| System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);

            return allFields
                .Where(f => f.FieldType == typeof(Step))
                .Select(f => f.GetValue(null))
                .Cast<Step>()
                .ToArray();
        }

        public void PrintAllResult()
        {
            Console.WriteLine($"UserCount = {UserCount}");

            var steps = GetAllSteps();

            foreach (var step in steps)
                step.PrintState();
        }
    }
}