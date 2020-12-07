using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using gRpcLinq2dbServer.DataSource.Connection;
using gRpcLinq2dbServer.ProductSpace;
using Microsoft.Extensions.Logging;

namespace gRpcLinq2dbServer.Services.ProductSpace
{
    public class ProductService: Product.ProductBase
    {
        private readonly ILogger<ProductService> _logger;

        public ProductService(ILogger<ProductService> logger)
        {
            _logger = logger;
        }

        public override Task<ProductInfoEntity> GetProductInfo(ProductInfoIdentity request, ServerCallContext context)
        {
            var dataConnection = DataBaseConnection.GetConnection("linq2dbTest");
            if (dataConnection == null)
                return Task.FromResult(GetEmpty(request, "<NO_CONNECTION>"));

            var result = dataConnection.Products
                .SingleOrDefault(p => p.Id == request.Id);
            if(result == null)
                return Task.FromResult(GetEmpty(request, "<NO_ENTITY>"));

            return Task.FromResult(new ProductInfoEntity
            {
                Id = result.Id,
                Name = result.Name
            });
        }

        private static ProductInfoEntity GetEmpty(ProductInfoIdentity request, string errorValue)
        {
            return new ProductInfoEntity
            {
                Id = request.Id,
                Name = errorValue
            };
        }
    }
}