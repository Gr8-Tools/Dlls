﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using gRpcLinq2dbServer.DataSource.Connection;
using gRpcLinq2dbServer.ProductSpace;
using LinqToDB;
using LinqToDB.Tools;
using Microsoft.Extensions.Logging;
using Product = gRpcLinq2dbServer.ProductSpace.Product;
using CategoryModel = gRpcLinq2dbServer.DataSource.Models.Category;
using ProductModel = gRpcLinq2dbServer.DataSource.Models.Product;

namespace gRpcLinq2dbServer.Services.ProductSpace
{
    //ToDo: https://github.com/grpc/grpc-dotnet/blob/master/examples/Mailer/Server/Services/MailerService.cs
    //ToDo: https://github.com/grpc/grpc/blob/v1.33.2/examples/csharp/RouteGuide/RouteGuideServer/RouteGuideImpl.cs
    public class ProductService: Product.ProductBase
    {
        private readonly ILogger<ProductService> _logger;
        private  DataBaseConnection? _dataConnection;

        public ProductService(ILogger<ProductService> logger)
        {
            _logger = logger;
            _dataConnection = DataBaseConnection.GetConnection("linq2dbTest");
        }

        public override Task<ProductInfoEntity> GetProductInfo(ProductInfoIdentity request, ServerCallContext context)
        {
            if (_dataConnection == null && (_dataConnection = DataBaseConnection.GetConnection("linq2dbTest")) == null)
                return Task.FromResult(GetEmpty(request, "<NO_CONNECTION>"));

            var result = _dataConnection.Products
                .SingleOrDefault(p => p.Id == request.Id);
            if(result == null)
                return Task.FromResult(GetEmpty(request, "<NO_ENTITY>"));

            return Task.FromResult(new ProductInfoEntity
            {
                Id = result.Id,
                Name = result.Name
            });
        }
        
        public override Task<ExtendedProductInfoEntity> GetExtendedProductInfo(ProductInfoIdentity request, ServerCallContext context)
        {
            if (_dataConnection == null && (_dataConnection = DataBaseConnection.GetConnection("linq2dbTest")) == null)
                return Task.FromResult(GetExtendedEmpty(request, "<NO_CONNECTION>"));

            var result = _dataConnection.Products
                .Where(p => p.Id == request.Id)
                .LoadWith(p => p.CategoryIdfKey)
                .SingleOrDefault();
            
            if(result == null)
                return Task.FromResult(GetExtendedEmpty(request, "<NO_ENTITY>"));

            return Task.FromResult(new ExtendedProductInfoEntity
            {
                Id = result.Id,
                Name = result.Name,
                CategoryInfo = new CategoryInfoEntity
                {
                    Id = result.CategoryIdfKey.Id,
                    Name = result.CategoryIdfKey.Name
                }
            });
        }

        /// <summary>
        /// EXAMPLE #1:
        /// Collecting all identity key's call base and stream results 
        /// </summary>
        public override async Task GetProductInfos(IAsyncStreamReader<ProductInfoIdentity> requestStream, IServerStreamWriter<ProductInfoEntity> responseStream, ServerCallContext context)
        {
            var list = new List<int>();
            while (await requestStream.MoveNext(context.CancellationToken))
            {
                if (_dataConnection == null && (_dataConnection = DataBaseConnection.GetConnection("linq2dbTest")) == null)
                {
                    await responseStream.WriteAsync(GetEmpty(requestStream.Current, "<NO_CONNECTION>"));
                    continue;
                }
                
                list.Add(requestStream.Current.Id);
            }

            if(!list.Any())
                return;
            
            list = list.Distinct().ToList();
            var results = await _dataConnection?.Products
                .Where(p => p.Id.In(list.Distinct()))
                .DefaultIfEmpty()
                .ToListAsync(context.CancellationToken)! ?? new List<ProductModel>();

            var notFoundProducts = list
                .Except(results.Select(r => r.Id))
                .Select(id => new DataSource.Models.Product
                {
                    Id = id,
                    Name = "<NO_ENTITY>"
                });
            results.AddRange(notFoundProducts);

            foreach (var result in results)
            {
                await responseStream.WriteAsync(new ProductInfoEntity
                {
                    Id = result.Id,
                    Name = result.Name
                });
            }
        }

        /// <summary>
        /// Example #2:
        /// Foreach input identity send information 
        /// </summary>
        public override async Task GetExtendedProductInfos(IAsyncStreamReader<ProductInfoIdentity> requestStream, IServerStreamWriter<ExtendedProductInfoEntity> responseStream,
            ServerCallContext context)
        {
            while (await requestStream.MoveNext(context.CancellationToken))
            {
                if (_dataConnection == null && (_dataConnection = DataBaseConnection.GetConnection("linq2dbTest")) == null)
                {
                    await responseStream.WriteAsync(GetExtendedEmpty(requestStream.Current, "<NO_CONNECTION>"));
                }
                else
                {
                    var result = _dataConnection.Products
                        .Where(p => p.Id == requestStream.Current.Id)
                        .LoadWith(p => p.CategoryIdfKey)
                        .SingleOrDefault();

                    if (result == null)
                        await responseStream.WriteAsync(GetExtendedEmpty(requestStream.Current.Id, "<NO_ENTITY>"));
                    else
                        await responseStream.WriteAsync(new ExtendedProductInfoEntity
                        {
                            Id = result.Id,
                            Name = result.Name,
                            CategoryInfo = new CategoryInfoEntity
                            {
                                Id = result.CategoryIdfKey.Id,
                                Name = result.CategoryIdfKey.Name
                            }
                        });
                }                
            }
        }

        public override async Task<CountEntity> SetExtendedProductInfos(IAsyncStreamReader<ExtendedProductInfoEntity> requestStream, ServerCallContext context)
        {
            uint count = 0;
            while (await requestStream.MoveNext())
            {
                if (_dataConnection == null && (_dataConnection = DataBaseConnection.GetConnection("linq2dbTest")) == null)
                    continue;

                var info = requestStream.Current;
                _dataConnection.Categories.InsertOrUpdate(
                    () => new CategoryModel
                    {
                        Id = info.CategoryInfo.Id,
                        Name = info.CategoryInfo.Name
                    },
                    c => new CategoryModel
                    {
                        Name = info.CategoryInfo.Name
                    });
                _dataConnection.Products.InsertOrUpdate(
                    () => new ProductModel
                    {
                        Id = info.Id,
                        Name = info.Name,
                        CategoryId = info.CategoryInfo.Id
                    },
                    p => new ProductModel
                    {
                        Name = info.Name,
                        CategoryId = info.CategoryInfo.Id
                    });
                count++;
            }

            return new CountEntity{ Count = count};
        }

        private static ProductInfoEntity GetEmpty(ProductInfoIdentity request, string errorValue)
        {
            return GetEmpty(request.Id, errorValue);
        }
        
        private static ExtendedProductInfoEntity GetExtendedEmpty(ProductInfoIdentity request, string errorValue)
        {
            return GetExtendedEmpty(request.Id, errorValue);
        }

        private static ProductInfoEntity GetEmpty(int id, string errorValue)
        {
            return new ProductInfoEntity
            {
                Id = id,
                Name = errorValue
            };
        }

        private static ExtendedProductInfoEntity GetExtendedEmpty(int id, string errorValue)
        {
            return new ExtendedProductInfoEntity
            {
                Id = id,
                Name = errorValue,
                CategoryInfo = new CategoryInfoEntity
                {
                    Id = 0,
                    Name = errorValue
                }
            };
        }
    }
}