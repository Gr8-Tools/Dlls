#nullable enable
using System;
using System.Collections.Generic;
using System.Configuration;
using linq2dbTest.Models;
using LinqToDB;
using LinqToDB.Configuration;
using LinqToDbApi.Connection;

namespace Linq2DbTest.Operations.Connections
{
    //ToDo: Write interface to autogenerate tables (m.b. in baseClass)
    public class TestDataConnection: BaseDataConnection
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
        
        public TestDataConnection() : base() { }
        
        public TestDataConnection(ConnectionStringSettings? configString): base(configString) { }

        public TestDataConnection(IConnectionStringSettings connectionString) : base(connectionString) { }
        
        #endregion
        
        /// <summary>
        /// Enumerable of ITable Types
        /// </summary>
        public override IEnumerable<Type> SelfInitialize => new []
        {
            typeof(Category),
            typeof(Product),
            typeof(Employee),
            typeof(City),
            typeof(Supplier),
            typeof(ProductsSuppliers)
        };

        /// <summary>
        /// Configuring connection
        /// </summary>
        public override void Configure()
        {
            AutoGenerate();
        }
    }
}