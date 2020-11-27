using System;
using System.Linq;
using linq2dbTest.Models;
using Linq2DbTest.Operations.Connections;
using Linq2DbTest.Settings;
using LinqToDbApi.Settings.Utils.ConnectionStringOptions;

namespace Linq2DbTest
{
    class Program
    {
        static void Main(string[] args)
        {
            TestConnection();
        }

        private static void TestConnection()
        {
            var options = new StandardNativeConnectionStringSettingsOptions("linq2dbTest", "postgres", "A!s2D#f4");
            var connectionSettings = new TestConnectionSettings(options);
            var connectionString = connectionSettings.ConnectionStrings.First();

            using (var db = new TestDataConnection(connectionString))
            {
                db.ConfigureExample();
                
                //Examples.InsertExample1(db);
                //Examples.InsertExample2(db);
                //Examples.InsertExample3(db);

                // foreach (var entity in db.ExampleSelectAll<Product>() ?? new Product[0])
                //     Console.WriteLine(entity.ToString());

                // foreach (var entity in db.ExampleSelectLeftJoin())
                //     Console.WriteLine(entity.ToString());
                
                // foreach (var entity in db.ExampleSelectAllPromParent())
                //     Console.WriteLine(entity.ToString());
            }
            
            Console.WriteLine("Finished!");
        }
    }
}