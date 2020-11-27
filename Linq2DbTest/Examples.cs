using System;
using System.Collections.Generic;
using System.Linq;
using linq2dbTest.Models;
using linq2dbTest.Models.Examples;
using Linq2DbTest.Operations.Connections;
using LinqToDB;
using LinqToDB.Common;
using LinqToDbApi.Connection;
using LinqToDbApi.Extensions;

namespace Linq2DbTest
{
    public static class Examples
    {
        /// <summary>
        /// Configure example
        /// </summary>
        public static void ConfigureExample(this BaseDataConnection db)
        {
            db.onExistedMapGet += dict =>
            {
                foreach (var (key, value) in dict)
                    Console.WriteLine($"{key.Name} exists? {value}");
            };
                
            db.onExistedTablesStructureNeedUpdate += pairs =>
            {
                foreach (var pair in pairs)
                    Console.WriteLine($"{pair.TableType.Name}: {pair.ColumnName}");
            };

            db.Configure();
        }

        /// <summary>
        /// Simple insert entity to DB
        /// </summary>
        public static void InsertExample1(this TestDataConnection db)
        {
            db.Categories.Insert(() => new Category {Name = "Category 1"});
            db.Categories.Insert(() => new Category {Name = "Category 2"});
        }

        /// <summary>
        /// Insert multiple Product-entities for each Category-entity
        /// WARNING: at this example result has no logic
        /// </summary>
        public static void InsertExample2(this TestDataConnection db)
        {
            db.Categories.Insert(
                db.Products,
                c => new Product {Name = "Product 1", CategoryId = c.Id});
        }
        
        /// <summary>
        /// Insert Product-entities, generated from selected data
        /// </summary>
        public static void InsertExample3(this TestDataConnection db)
        {
            var category1 = db.Categories
                .FirstAsync(c => c.Name == "Category 1")
                .GetAwaiter().GetResult();

            var category2Id = db.Categories
                .Where(c => c.Name == "Category 2")
                .Select(c => c.Id)
                .ToArray().FirstOrDefault();

            db.Products.Insert(() => new Product {Name = "Product 1", CategoryId = category1.Id});
            db.Products.Insert(() => new Product {Name = "Product 2", CategoryId = category2Id});
        }

        /// <summary>
        /// Select all entities and columns from Table 
        /// </summary>
        public static IEnumerable<T>? ExampleSelectAll<T>(this BaseDataConnection db) where T : class
        {
            return !typeof(T).IsTable() 
                ? null : 
                db.GetTable<T>().ToList();
        }
        
        /// <summary>
        /// Left join Category-Products
        /// Result: LeftJoinProduct
        /// </summary>
        public static IEnumerable<LeftJoinProduct> ExampleSelectLeftJoin(this TestDataConnection db)
        {
            return (from c in db.Categories
                from p in db.Products.Where(p => p.CategoryId == c.Id)
                orderby c.Name, p.Id
                select LeftJoinProduct.Build(p, c)).ToList();
        }

        /// <summary>
        /// Selecting all Product from Category
        /// WARNING: Needs AllowMultipleQuery = true
        /// </summary>
        public static IEnumerable<Product> ExampleSelectAllPromParent(this TestDataConnection db)
        {
            Configuration.Linq.AllowMultipleQuery = true;
            
            var result = db.Categories
                .Where(c => c.Name == "Category 1")
                .LoadWith(c => c.ProductCategoryIdfKeys)
                .ToArray();

            return result.FirstOrDefault()?.ProductCategoryIdfKeys ?? new Product[0];
        }
    }
}