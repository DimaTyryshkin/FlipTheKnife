using System;
using System.Collections.Generic;
using System.Linq;

namespace IntegrationTestInfrastructure
{
    public class TestOptions
    {
        public class Step
        {
            public string name;

            int counter;
            int errorCounter;
            public int overrideValueToSuccess;

            int GetGountToSuccess(int counterValueToSuccess)
            {
                return overrideValueToSuccess != 0 ? overrideValueToSuccess : counterValueToSuccess;
            }

            HashSet<string> existingValues = new HashSet<string>();

            /// <summary>
            /// Добавляет счетчик если <paramref name="hashValue"/> впервые добавялется
            /// </summary>
            /// <param name="hashValue"></param>
            public void IncrimentCounter(string hashValue)
            {
                if (!existingValues.Contains(hashValue))
                {
                    counter++;
                    existingValues.Add(hashValue);
                }
                else
                {
                    errorCounter++;
                }
            }

            public bool IsSuccess(int counterValueToSuccess)
            {
                return GetGountToSuccess(counterValueToSuccess) == counter;
            }

            public void PrintState(int counterValueToSuccess)
            {
                int cToSuccess = GetGountToSuccess(counterValueToSuccess);

                Console.WriteLine((IsSuccess(cToSuccess) ? stepSucces : stepFail) + $" : ({counter}/{cToSuccess}) errors: {errorCounter} {name}");
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

            foreach (var step in steps.Where(s => !s.IsSuccess(UserCount)))
                step.PrintState(UserCount);

            return steps.All(s => s.IsSuccess(UserCount));
        }

        public Step[] GetAllSteps()
        {
            var allFields = GetType().GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);

            return allFields
                .Where(f => f.FieldType == typeof(Step))
                .Select(f => f.GetValue(this))
                .Cast<Step>()
                .ToArray();
        }

        public void PrintAllResult()
        {
            Console.WriteLine("-------------------------------------");
            Console.WriteLine($"UserCount = {UserCount}");

            var steps = GetAllSteps();

            foreach (var step in steps)
                step.PrintState(UserCount);
        }
    }
}