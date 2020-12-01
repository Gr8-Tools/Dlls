using System;
using System.Collections.Generic;
using System.Linq;
using linq2dbTest.Models;
using linq2dbTest.Models.Examples;
using Linq2DbTest.Operations.Connections;
using Linq2DbTest.Settings;
using LinqToDB;
using LinqToDB.Common;
using LinqToDB.Data;
using LinqToDB.Tools;
using LinqToDbApi.Connection;
using LinqToDbApi.Extensions;

namespace Linq2DbTest.Operations
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

        public static void InsertOrUpdateExample4(this TestDataConnection db)
        {
            var randomizer = new Random();

            var productSupplierPairs = (
                from p in db.Products
                from s in db.Suppliers
                select new
                {
                    Id = Sql.Ext.RowNumber().Over().ToValue(),
                    ProductId = p.Id,
                    SupplierId = s.Id
                }).AsCte("AllProductSupplierPairs");

            var pairsCount = (
                from ps in productSupplierPairs
                select new
                {
                    Count = Sql.Ext.Count(ps.Id, Sql.AggregateModifier.All).ToValue()
                }).SingleOrDefault()?.Count ?? 0;

            if (pairsCount == 0)
                return;
            
            var selectedRowsCount = randomizer.Next(1, pairsCount);
            var selectedRowsIds = Enumerable.Range(1, selectedRowsCount)
                .Select(i => (long)randomizer.Next(1, pairsCount))
                .Distinct().ToArray();

            var selectedPairs = productSupplierPairs
                .Where(ps => ps.Id.In(selectedRowsIds))
                .Select(ap => new {ap.ProductId, ap.SupplierId})
                .AsCte("SelectedProductSupplierPairs");

            foreach (var result in selectedPairs.Select(s => s).ToArray())
                Console.WriteLine($"Product [{result.ProductId}] <-> Supplier [{result.SupplierId}]");

            var newPairs = (
                    from sp in selectedPairs
                    from psm in db.ProductsSuppliersMap.LeftJoin(ps =>
                        ps.ProductId == sp.ProductId && ps.SupplierId == sp.SupplierId)
                    select new
                    {
                        ProductId = sp.ProductId,
                        SupplierId = sp.SupplierId,
                        ExistedProductdId = psm.ProductId,
                        ExistedSupplierId = psm.SupplierId
                    }).ToArray()
                .Where(e => e.ExistedProductdId == 0 && e.ExistedSupplierId == 0)
                .Select(e => new ProductsSuppliers {ProductId = e.ProductId, SupplierId = e.SupplierId})
                .ToArray();
            
            foreach (var result in newPairs)
                Console.WriteLine($"Product [{result.ProductId}] <-> Supplier [{result.SupplierId}]");
            
            db.ProductsSuppliersMap.BulkCopy(newPairs);
        }

        /// <summary>
        /// Insert Multiple-Product-entities< generated from Category-entities 
        /// </summary>
        public static void BulkCopyExample(this TestDataConnection db)
        {
            var categories = db.ExampleSelectAll<Category>() ?? new Category[0];
            var range = Enumerable.Range(1, 3);
            var products = categories
                .Select(c => 
                    range.Select(r =>
                        new Product
                        {
                            CategoryId = c.Id,
                            Name = $"Product {c.Name.Split(' ')[1]}.{r}"
                        }
                    ))
                .SelectMany(p => p);

            db.Products.BulkCopy(products);
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
        /// Inner join Category-Products
        /// Result: InnerJoinProduct
        /// </summary>
        public static IEnumerable<JoinProduct> ExampleSelectInnerJoin(this TestDataConnection db)
        {
            return (from c in db.Categories
                from p in db.Products.InnerJoin(p => p.CategoryId == c.Id)
                //from p in db.Products.Where(p => p.CategoryId == c.Id)
                orderby c.Name, p.Id
                select JoinProduct.Build(p, c)).ToList();
        }

        /// <summary>
        /// Left join Category-Products
        /// Result: LeftJoinProduct
        /// </summary>
        public static IEnumerable<JoinProduct> ExampleSelectLeftJoin(this TestDataConnection db)
        {
            return (
                from c in db.Categories
                from p in db.Products.LeftJoin(p => p.CategoryId == c.Id)
                //from p in db.Products.Where(p => p.CategoryId == c.Id).DefaultIfEmpty()
                select JoinProduct.Build(p, c)).ToList();
        }
        
        /// <summary>
        /// Left join Category-Products
        /// Result: FullJoinProduct
        /// </summary>
        public static IEnumerable<JoinProduct> ExampleSelectFullJoin(this TestDataConnection db)
        {
            return (
                from c in db.Categories
                from p in db.Products.FullJoin(p => p.CategoryId == c.Id)
                //from p in db.Products.Where(p => p.CategoryId == c.Id).DefaultIfEmpty()
                select JoinProduct.Build(p, c)).ToList();
        }

        /// <summary>
        /// Selecting all Product from Category
        /// WARNING: Needs AllowMultipleQuery = true
        /// </summary>
        public static IEnumerable<Product> ExampleSelectAllFromParent(this TestDataConnection db)
        {
            Configuration.Linq.AllowMultipleQuery = true;
            
            var result = db.Categories
                .Where(c => c.Name == "Category 1")
                .LoadWith(c => c.ProductCategoryIdfKeys)
                .ToArray();

            return result.FirstOrDefault()?.ProductCategoryIdfKeys ?? new Product[0];
        }

        /// <summary>
        /// Select Multi-Products to Multi-Supplier
        /// </summary>
        public static void SelectMultiToMulti(this TestDataConnection db)
        {
            var results = (
                from p in db.Products
                from s in db.Suppliers
                from ps in db.ProductsSuppliersMap.InnerJoin(ps => ps.ProductId == p.Id && ps.SupplierId == s.Id)
                from city in db.Cities.InnerJoin(c => c.Id == s.CityId)
                from category in db.Categories.InnerJoin(c => c.Id == p.CategoryId)
                select new
                {
                    ProductId = p.Id,
                    ProductName = p.Name,
                    CategoryName = category.Name,
                    SupplierId = s.Id,
                    SupplierName = s.Name,
                    CityName = city.Name
                }
            )
                .OrderBy(r => r.ProductId).ThenBy(r => r.SupplierId)
                .ToArray();

            foreach (var result in results)
            {
                Console.WriteLine($"Product [{result.ProductId}:\"{result.ProductName}\"] from Category \"{result.CategoryName}\" supplied by [{result.SupplierId}:\"{result.SupplierName}\"] from City \"{result.CityName}\"");
            }
        }

        /// <summary>
        /// Updating FirstName and LastName of random selected
        /// </summary>
        public static void ExampleUpdate(this TestDataConnection db)
        {
            Configuration.Linq.AllowMultipleQuery = true;
            
            var randomizer = new Random();
            var randomIds = Enumerable
                .Range(0, randomizer.Next(3,5))
                .Select(r => randomizer.Next(1, 11))
                .Distinct().ToArray();

            var randomEmployees = db.Employees
                .Where(e => e.Id.In(randomIds))
                .Select(e => e);
            var selectedEmployees = randomEmployees.LoadWith(e => e.EmployeesSubordinate).ToArray();
            foreach (var employee in selectedEmployees)
                Console.WriteLine(employee);

            randomEmployees
                .Set(e => e.FirstName, e => CteExamples.Names[randomizer.Next(CteExamples.Names.Length)])
                .Set(e => e.LastName, e => CteExamples.Surnames[randomizer.Next(CteExamples.Surnames.Length)])
                .Update();

            Console.WriteLine("After Update");
            
            selectedEmployees = randomEmployees.LoadWith(e => e.EmployeesSubordinate).ToArray();
            foreach (var employee in selectedEmployees)
                Console.WriteLine(employee);
            
            Configuration.Linq.AllowMultipleQuery = false;
        }

        /// <summary>
        /// Updating FirstName and LastName of specify selected
        /// </summary>
        public static void ExampleUpdate2(this TestDataConnection db, params int[] employeeIds)//3,5,6
        {
            Configuration.Linq.AllowMultipleQuery = true;
            
            var randomizer = new Random();
            
            var selectedEmployeeQuery = db.Employees
                .Where(e => e.Id.In(employeeIds)).LoadWith(e => e.EmployeesSubordinate);
            foreach (var employee in selectedEmployeeQuery.ToArray())
            {
                Console.WriteLine(employee.ToString());
                db.Employees.Update(
                    e => e.Id == employee.Id,
                    e => new Employee
                    {
                        FirstName = CteExamples.Names[randomizer.Next(CteExamples.Names.Length)],
                        LastName = CteExamples.Surnames[randomizer.Next(CteExamples.Surnames.Length)]
                    });
            }
            
            Console.WriteLine("After update: ");
            foreach (var employee in selectedEmployeeQuery.ToArray())
                Console.WriteLine(employee.ToString());
            
            Configuration.Linq.AllowMultipleQuery = false;
        }
    }
}