using System;
using System.Diagnostics;
using System.Linq;
using Linq2DbTest.Operations;
using Linq2DbTest.Operations.Connections;
using Linq2DbTest.Settings;
using LinqToDB.Data;
using LinqToDbApi.Settings;
using LinqToDbApi.Settings.Utils;
using LinqToDbApi.Settings.Utils.ConnectionStringOptions;

namespace Linq2DbTest
{
    class Program
    {
        static void Main(string[] args)
        {
            TestConnection();
        }

        //ToDo: Add City table, association with Supplier and N:N assosiasitons for Product_Supplier (m.b, with external Table) 
        private static void TestConnection()
        {
            #if DEBUG
            DataConnection.TurnTraceSwitchOn();
            DataConnection.WriteTraceLine = (m, c, l) => Debugger.Log((int)l,c, m); 
            #endif
            
            
            //var options = new StandardNativeConnectionStringSettingsOptions("linq2dbTest", "postgres", "<ARMY_PASSWORD>");

            var options = NativeConnectionSettings.GenerateAllOptions(
                new NativeConnectionStringSettingsOptionsData
                {
                    DataBase = "linq2dbTest",
                    UserId = "postgres",
                    Password = "<ARMY_PASSWORD",
                    CommandTimeout = 10,
                    ConnectionTimeout = 10,
                    Protocol = 3,
                    MinPoolSize = 10,
                    MaxPoolSize = 100,
                    ConnectionLifeTime = 10
                }).ToArray();
            
            var connectionSettings = new TestConnectionSettings(options);
            var connectionString = connectionSettings.Get(NpgsqlConnectionTypes.Standard);

            if (connectionString == null)
                throw new Exception($"Connection string of type [{typeof(StandardNativeConnectionStringSettingsOptions)}] is not defined");

            using (var db = new TestDataConnection(connectionString))
            {
                //db.ConfigureExample();
                
                //Examples.InsertExample1(db);
                //Examples.InsertExample2(db);
                //Examples.InsertExample3(db);
                //db.InsertOrUpdateExample4();

                //db.BulkCopyExample();

                // foreach (var entity in db.ExampleSelectAll<Product>() ?? new Product[0])
                //     Console.WriteLine(entity.ToString());

                // Console.WriteLine("Inner join:");
                // foreach (var entity in db.ExampleSelectInnerJoin())
                //     Console.WriteLine(entity.ToString());

                // Console.WriteLine("Left join:");
                // foreach (var entity in db.ExampleSelectLeftJoin())
                //     Console.WriteLine(entity.ToString());

                // Console.WriteLine("Full join:");
                // foreach (var entity in db.ExampleSelectFullJoin())
                //     Console.WriteLine(entity.ToString());

                // foreach (var entity in db.ExampleSelectAllFromParent())
                //     Console.WriteLine(entity.ToString());
                
                //db.SelectMultiToMulti();

                // //CTE
                // CteExamples.GenerateNewLevelEmployees(db);
                // CteExamples.LoadEmployeesOfLevelWithFullAssociations(db);

                // db.ExampleUpdate();
                // db.ExampleUpdate2(3, 5, 6);
            }
            
            Console.WriteLine("Finished!");
        }
    }
}