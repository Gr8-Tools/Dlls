using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using gRpcLinq2dbServer.DataSource.Connection;
using LinqToDbApi.Settings;
using LinqToDbApi.Settings.Utils;
using LinqToDbApi.Settings.Utils.ConnectionStringOptions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace gRpcLinq2dbServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (var db = ConnectToDB())
            {
                CreateHostBuilder(args).Build().Run();    
            }
        }

        // Additional configuration is required to successfully run gRPC on macOS.
        // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        private static DataBaseConnection ConnectToDB()
        {
            var options = NativeConnectionSettings.GenerateAllOptions(
                new NativeConnectionStringSettingsOptionsData
                {
                    DataBase = "linq2dbTest",
                    UserId = "postgres",
                    Password = "A!s2D#f4"
                }).ToArray();
            
            var connectionSettings = new ConnectionSettings(options);
            var connectionString = connectionSettings.Get(NpgsqlConnectionTypes.Standard);

            if (connectionString == null)
                throw new Exception($"Connection string of type [{typeof(StandardNativeConnectionStringSettingsOptions)}] is not defined");
            
            return new DataBaseConnection(connectionString);
        }
    }
}
