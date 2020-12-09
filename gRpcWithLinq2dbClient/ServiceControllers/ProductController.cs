using System;
using System.Collections.Generic;
using System.Linq;
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

        #region OPERATIONS
        
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
        
        private async Task<IEnumerable<ProductInfoEntity>> GetSimpleInfos(int? minValue, int? maxValue, params int?[] ids)
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

        
        private async Task<IEnumerable<ExtendedProductInfoEntity>> GetExtendedInfos(int? minValue, int? maxValue, params int?[] ids)
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
        
        #endregion
        
        #region TRANSLATE COMMANDS

        /// <summary>
        /// navigate by command parts and do action 
        /// </summary>
        /// <param name="commandParts">
        /// [0] - "p"
        /// [1] - "get"/"set"
        /// [2] - "simple"/"extended"
        /// [3] - "single"/"stream"
        /// [4] - arguments
        /// </param>
        /// <returns>Result, if command is known</returns>
        public bool TranslateCommand(in string[] commandParts)
        {
            if (commandParts.Length < 5)
                return false;
            
            switch (commandParts[1])
            {
                case "get":
                    return TranslateGetCommand(in commandParts);
                case "set":
                    return TranslateSetCommand(in commandParts);
            }

            return false;
        }

        private bool TranslateGetCommand(in string[] commandParts)
        {
            int? minValue, maxValue;
            switch (commandParts[3])
            {
                case "single":
                    ConvertGetSingleArguments(commandParts[4], out var id, out minValue, out maxValue);
                    switch (commandParts[2])
                    {
                        case "simple":
                            Console.WriteLine(GetSimpleInfo(id,minValue,maxValue).GetAwaiter().GetResult());
                            return true;
                        case "extended":
                            Console.WriteLine(GetExtendedInfo(id,minValue,maxValue).GetAwaiter().GetResult());
                            return true;
                    }
                    break;
                case "stream":
                    ConvertGetStreamArguments(commandParts[4], out var ids, out minValue, out maxValue);
                    switch (commandParts[2])
                    {
                        case "simple":
                            var simpleResults = GetSimpleInfos(minValue, maxValue, ids).GetAwaiter().GetResult();
                            foreach (var result in simpleResults)
                                Console.WriteLine(result);
                            Console.WriteLine();
                            return true;
                        case "extended":
                            var extendedResults = GetExtendedInfos(minValue,maxValue, ids).GetAwaiter().GetResult();
                            foreach (var result in extendedResults)
                                Console.WriteLine(result);
                            return true;
                    }
                    break;
            }
            
            return false;
        }
        
        private bool TranslateSetCommand(in string[] commandParts)
        {
            switch (commandParts[2])
            {
                case "simple":
                    throw new Exception("\"Simple\" requests for SET operation are not supported!");
                case "extended":
                    if(commandParts[3] == "single")
                        throw new Exception("SET \"Single\" entity is not supported!");
                    else if (commandParts[3] == "stream")
                    {
                        var args = ConvertSetMethodArguments(commandParts[4]);
                        Console.WriteLine($"Inserted count: [{SetExtendedInfos(args).GetAwaiter().GetResult()}]");
                        return true;
                    }
                    break;
            }
            return false;
        }

        #endregion
        
        public static ProductController Create(GrpcChannel channel)
        {
            return Instance ??= new ProductController(channel);
        }
        
        #region CONVERT_ARGUMENTS

        private static void ConvertGetSingleArguments(string argument, out int? id, out int? minValue,
            out int? maxValue)
        {
            var args = argument.Split(';');
            id = args.Length > 0 
                ? string.IsNullOrWhiteSpace(args[0]) ? (int?) null : int.Parse(args[0]) 
                : null;
            minValue = args.Length > 1
                ? string.IsNullOrWhiteSpace(args[1]) ? (int?) null : int.Parse(args[1])
                : null;
            maxValue = args.Length > 2
                ? string.IsNullOrWhiteSpace(args[2]) ? (int?) null : int.Parse(args[2])
                : null;
        }
        
        private static void ConvertGetStreamArguments(string argument, out int?[] ids, out int? minValue,
            out int? maxValue)
        {
            var args = argument.Split(';');
            minValue = args.Length > 0  
                ? string.IsNullOrWhiteSpace(args[0]) ? (int?) null : int.Parse(args[0]) 
                : null;
            maxValue = args.Length > 1 
                ? string.IsNullOrWhiteSpace(args[1]) ? (int?) null : int.Parse(args[1])
                : null;

            ids = null;
            if (args.Length >= 3 && !string.IsNullOrWhiteSpace(args[2]))
            {
                ids = args[2].Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(strId => (int?)int.Parse(strId))
                    .ToArray();
            }
        }

        private static IEnumerable<KeyValuePair<KeyValuePair<int, string>, KeyValuePair<int, string>>>
            ConvertSetMethodArguments(string argument)
        {
            var list = new List<KeyValuePair<KeyValuePair<int, string>, KeyValuePair<int, string>>>();
            if (string.IsNullOrWhiteSpace(argument))
                return list;

            var pairArgs = argument.Split(';');
            if (pairArgs.Length == 0)
                return list;

            list.AddRange(
                from pairArg in pairArgs 
                select pairArg.Split(',', StringSplitOptions.RemoveEmptyEntries) into args 
                where args.Length == 4 
                select new KeyValuePair<KeyValuePair<int, string>, KeyValuePair<int, string>>(
                    new KeyValuePair<int, string>(int.Parse(args[0]), args[1]), 
                    new KeyValuePair<int, string>(int.Parse(args[2]), args[3]))
            );
            return list;
        }

        #endregion
        
        private static IEnumerator<ProductInfoIdentity> GenerateIdentities(int? minValue, int? maxValue, params int?[] ids)
        {
            if (ids == null || ids.Length == 0)
            {
                yield return new ProductInfoIdentity {Id = RandomValue(minValue, maxValue ?? 100)};
                yield break;
            }
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