using System;
using System.Collections.Generic;
using gRpcLinq2dbServer.DataSource.Models;
using LinqToDbApi.Connection;

namespace gRpcLinq2dbServer.DataSource.Connection
{
    public class DataBaseConnection: BaseDataConnection
    {
        public override IEnumerable<Type> SelfInitialize => new[]
        {
            typeof(Category),
            typeof(Product),
            typeof(City),
            typeof(Employee),
            typeof(Supplier),
            typeof(ProductsSuppliers)
        };
        
        public override void Configure()
        {
            AutoGenerate();
        }
    }
}