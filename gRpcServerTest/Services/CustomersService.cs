using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace gRpcServerTest.Services
{
    public class CustomerService: Customer.CustomerBase
    {
        private readonly ILogger<CustomerService> _logger;

        public CustomerService(ILogger<CustomerService> logger)
        {
            _logger = logger;
        }

        public override Task<CustomerModel> GetCustomerInfo(CustomerLookupModel request, ServerCallContext context)
        {
            var output = new CustomerModel();

            switch (request.UserId)
            {
                case 1:
                    output.FirstName = "Jamie";
                    output.LastName = "Smith";
                    break;
                case 2:
                    output.FirstName = "Jane";
                    output.LastName = "Doe";
                    break;
                case 3:
                    output.FirstName = "Greg";
                    output.LastName = "Thomas";
                    break;
                default:
                    output.FirstName = "NOT DEFINED!";
                    output.LastName = "NOT DEFINED!";
                    break;
            }

            return Task.FromResult(output);
        }

        public override async Task GetNewCustomers(NewCustomerRequest request, IServerStreamWriter<CustomerModel> responseStream, ServerCallContext context)
        {
            List<CustomerModel> customers = new List<CustomerModel>
            {
                new CustomerModel
                {
                    FirstName = "Andrew",
                    LastName = "Kuzin",
                    EmailAddress = "email",
                    Age = 24,
                    IsAlive = true
                },
                new CustomerModel
                {
                    FirstName = "Steven",
                    LastName = "Hocking",
                    EmailAddress = "sh_email",
                    Age = 100,
                    IsAlive = false
                }
            };

            foreach (var customer in customers)
                await responseStream.WriteAsync(customer);
        }
    }
}