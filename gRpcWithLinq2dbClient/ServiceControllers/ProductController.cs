﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using gRpcLinq2dbServer.ProductSpace;

namespace gRpcWithLinq2dbClient.ServiceControllers
{
    //ToDo: https://github.com/grpc/grpc-dotnet/blob/master/examples/Mailer/Client/Program.cs
    //ToDo: https://github.com/grpc/grpc/blob/v1.33.2/examples/csharp/RouteGuide/RouteGuideClient/Program.cs
    public class ProductController
    {
        private const int MIN_RANDOM_VALUE = 1;
        private const int MAX_RANDOM_VALUE = 1;
        
        public static ProductController Instance { get; private set; }
        private static readonly Random Randomizer = new Random();
        
        private Product.ProductClient _client;
        
        private ProductController(ChannelBase channel)
        {
            _client = new Product.ProductClient(channel);
        }

        private async Task<ProductInfoEntity> GetSimpleInfo(int? id, int? minValue, int? maxValue)
        {
            IEnumerator<ProductInfoIdentity> identities;
            if ((identities = GenerateIdentities(minValue, maxValue, id)).MoveNext())
                return await _client.GetProductInfoAsync(identities.Current);

            return null;
        }
        
        private async Task<ExtendedProductInfoEntity> GetExtendedInfo(int? id, int? minValue, int? maxValue)
        {
            IEnumerator<ProductInfoIdentity> identities;
            if ((identities = GenerateIdentities(minValue, maxValue, id)).MoveNext())
                return await _client.GetExtendedProductInfoAsync(identities.Current);

            return null;
        }
        
        private async Task<IEnumerable<ProductInfoEntity>> GetSimpleInfos(
            int? minValue, int? maxValue, params int?[] ids)
        {
            var list = new List<ProductInfoEntity>();
            var identities = GenerateIdentities(minValue, maxValue, ids);
            using (var call = _client.GetProductInfos())
            {
                while (identities.MoveNext())
                    await call.RequestStream.WriteAsync(identities.Current);

                await call.RequestStream.CompleteAsync();

                while (await call.ResponseStream.MoveNext())
                    list.Add(call.ResponseStream.Current);
            }

            return list;
        }

        
        private async Task<IEnumerable<ExtendedProductInfoEntity>> GetExtendedInfos(
            int? minValue, int? maxValue, params int?[] ids)
        {
            var list = new List<ExtendedProductInfoEntity>();
            var identities = GenerateIdentities(minValue, maxValue, ids);
            using (var call = _client.GetExtendedProductInfos())
            {
                var responseTask = Task.Run(async () =>
                {
                    while (await call.ResponseStream.MoveNext())
                        list.Add(call.ResponseStream.Current);
                });

                while (identities.MoveNext())
                    await call.RequestStream.WriteAsync(identities.Current);

                await call.RequestStream.CompleteAsync();
                await responseTask;
            }

            return list;
        }

        private async Task<uint> SetExtendedInfos(IEnumerable<KeyValuePair<KeyValuePair<int, string>, KeyValuePair<int, string>>> newItems)
        {
            using (var call = _client.SetExtendedProductInfos())
            {
                foreach (var (product, category) in newItems)
                {
                    var request = new ExtendedProductInfoEntity
                    {
                        Id = product.Key,
                        Name = product.Value,
                        CategoryInfo = new CategoryInfoEntity
                        {
                            Id = category.Key,
                            Name = category.Value
                        }
                    };
                    await call.RequestStream.WriteAsync(request);
                }
                await call.RequestStream.CompleteAsync();
                
                var result = await call.ResponseAsync;
                return result.Count;
            }
        }
        
        public static ProductController Create(GrpcChannel channel)
        {
            return Instance ??= new ProductController(channel);
        }

        /// <summary>
        /// navigate by command parts and do action 
        /// </summary>
        /// <param name="commandParts">
        /// [0] - "p"
        /// [1] - "get"/"set"
        /// [2] - "simple"/"extended"
        /// [3] - "single"/"stream"
        /// </param>
        /// <returns>Result, if command is known</returns>
        public static bool TranslateCommand(in string[] commandParts)
        {
            switch (commandParts[1])
            {
                case "get":
                    return TranslateGetCommand(in commandParts);
                case "set":
                    return TranslateSetCommand(in commandParts);
            }

            return false;
        }

        private static bool TranslateGetCommand(in string[] commandParts)
        {
            switch (commandParts[2])
            {
                case "simple":
                    return true;
                case "extended":
                    return true;
            }

            return false;
        }

        private static bool TranslateSetCommand(in string[] commandParts)
        {
            switch (commandParts[2])
            {
                case "simple":
                    throw new Exception("\"Simple\" requests for SET operation are not supported!");
                case "extended":
                    if(commandParts[3] == "single")
                        throw new Exception("SET \"Single\" entity is not supported!");
                    return true;
            }

            return false;
        }
        
        private static IEnumerator<ProductInfoIdentity> GenerateIdentities(int? minValue, int? maxValue, params int?[] ids)
        {
            if (ids.Length == 0)
                yield return new ProductInfoIdentity {Id = RandomValue(minValue, maxValue ?? 100)};
            foreach (var id in ids)
                yield return new ProductInfoIdentity {Id = id ?? RandomValue(minValue, maxValue, ids.Length == 1 ? 100 : ids.Length)};
        }

        private static int RandomValue(int? minValue, int? maxValue, int count = 1)
        {
            return Randomizer.Next(
                minValue ?? MIN_RANDOM_VALUE,
                maxValue ?? MAX_RANDOM_VALUE * count);
        }
    }
}