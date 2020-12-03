using System;
using System.Threading.Tasks;
using Grpc.Net.Client;
using gRpcLinq2dbServer.ProductSpace;

namespace gRpcWithLinq2dbClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var chanel = GrpcChannel.ForAddress("https://localhost:5001");
            
            // var client = new Product.ProductClient(chanel);
            // var input = new ProductInfoRequest {Id = 1};
            // var reply = await client.GetProductInfoAsync(input);
            //
            // Console.WriteLine(reply);
            
            Console.ReadLine();
        }
    }
}