using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Net.Client;

namespace gRpcClientTest
{
    class Program
    { 
        private static async Task Main(string[] args)
        {
            var channel = GrpcChannel.ForAddress("https://localhost:5002");
            
            // var client = new Greeter.GreeterClient(channel);
            // var input = new HelloRequest {Name = "Andrew"};
            // var reply = await client.SayHelloAsync(input);
            //
            // Console.WriteLine(reply.Message);
            
            var client = new Customer.CustomerClient(channel);
            var input = new CustomerLookupModel {UserId = 4};
            var reply = await client.GetCustomerInfoAsync(input);
            
            Console.WriteLine(reply.ToString());

            using (var call = client.GetNewCustomers(new NewCustomerRequest()))
            {
                while (await call.ResponseStream.MoveNext(CancellationToken.None))
                {
                    var newCustomer = call.ResponseStream.Current;
                    Console.WriteLine(newCustomer.ToString());
                }
            }
            
            Console.ReadLine();
        }
    }
}