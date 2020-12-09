using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using gRpcLinq2dbServer.DataSource.Models;
using LinqToDB;
using LinqToDB.Configuration;
using LinqToDbApi.Connection;

namespace gRpcLinq2dbServer.DataSource.Connection
{
    public class DataBaseConnection: BaseDataConnection
    {
        #region TABLES
        
        public ITable<Product> Products => GetTable<Product>();
        
        public ITable<Category> Categories => GetTable<Category>();

        public ITable<Employee> Employees => GetTable<Employee>();

        public ITable<City> Cities => GetTable<City>();

        public ITable<Supplier> Suppliers => GetTable<Supplier>();

        public ITable<ProductsSuppliers> ProductsSuppliersMap => GetTable<ProductsSuppliers>();

        #endregion
        
        #region CONSTRUCTORS
        
        public DataBaseConnection() : base() { }
        
        public DataBaseConnection(ConnectionStringSettings? configString): base(configString) { }
        public DataBaseConnection(IConnectionStringSettings connectionString) : base(connectionString) { }
        
        #endregion
        
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
        
        public new static DataBaseConnection? GetConnection(string database)
        {
            return (DataBaseConnection) BaseDataConnection.GetConnection(database)!;
        } 
    }
}