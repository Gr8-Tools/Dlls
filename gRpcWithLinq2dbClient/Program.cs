using System;
using System.Threading.Tasks;
using Grpc.Net.Client;
using gRpcWithLinq2dbClient.ServiceControllers;

namespace gRpcWithLinq2dbClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var chanel = GrpcChannel.ForAddress("https://localhost:5001");
            CreateServiceControllers(chanel);

            var command = Console.ReadLine();
            while (command != "exit")
            {
                if(!TranslateCommand(command))
                    Console.WriteLine($"Unknown command \"{command}\"");
                    
                command = Console.ReadLine();
            }
        }

        private static void CreateServiceControllers(GrpcChannel chanel)
        {
            ProductController.Create(chanel);
        }

        private static bool TranslateCommand(in string command)
        {
            var commandParts = command.Split('.');
            if (commandParts.Length == 0)
                return false;

            switch (commandParts[0])
            {
                case "p":
                    return ProductController.TranslateCommand(in commandParts);
            }
            
            return false;
        }
    }
}