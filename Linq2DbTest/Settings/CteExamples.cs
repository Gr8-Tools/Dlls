using System;
using System.Collections.Generic;
using System.Linq;
using linq2dbTest.Models;
using Linq2DbTest.Operations.Connections;
using LinqToDB;
using LinqToDB.Common;
using LinqToDB.Data;

namespace Linq2DbTest.Settings
{
    public static class CteExamples
    {
        public static readonly string[] Names = 
        {
            "Alisa", "Bob", "Charles", "Denis", "Emma",
            "Frank", "Georgy", "Hank", "Isaak", "John",
            "Karl", "Luis", "Max", "Nicole", "Ora"
        };

        public static readonly string[] Surnames =
        {
            "Wade", "Netherwood", "Morris", "Xavier", "Morrison",
            "Wilkins", "Jefferson", "Miller", "Wakeford", "Wrigley", "York",
            "Perkins", "Chen", "Mitchell", "Barnes", "Williams", "Thayer"
        };
        
        /// <summary>
        /// Generate Common Table Expression
        /// </summary>
        public static IQueryable<EmployeeHierarchyCte> GetHierarchyEmployeesCte(this TestDataConnection db)
        {
            return
                db.GetCte<EmployeeHierarchyCte>(employeeHierarchy =>
                {
                    return (
                        from e in db.Employees
                        where e.ManagerId == null
                        select new EmployeeHierarchyCte
                        {
                            Id = e.Id,
                            FirstName = e.FirstName,
                            LastName = e.LastName,
                            ManagerId = e.ManagerId,
                            HierarchyLevel = 1
                        })
                    .Concat(
                        from e in db.Employees
                        from eh in employeeHierarchy
                            .InnerJoin(eh => e.ManagerId == eh.Id)
                        select new EmployeeHierarchyCte
                        {
                            Id = e.Id,
                            FirstName = e.FirstName,
                            LastName = e.LastName,
                            ManagerId = e.ManagerId,
                            HierarchyLevel = eh.HierarchyLevel + 1
                        });
                });
        }
        
        /// <summary>
        /// Using CTE find Max in next query  
        /// </summary>
        public static int GetMaxLevel(IQueryable<EmployeeHierarchyCte> hierarchyQuery)
        {
            return
                (
                    from eh in hierarchyQuery
                    select new
                    {
                        Value = Sql.Ext.Max(eh.HierarchyLevel)
                            .Over()
                            .ToValue()
                    }).FirstOrDefault()?.Value ?? 0;
        }

        /// <summary>
        /// Creating Root elements of Employee table 
        /// </summary>
        private static IEnumerable<Employee> GenerateRootManagers(this TestDataConnection db)
        {
            var randomizer = new Random();

            var managerIds = Enumerable.Range(1, 5).Select(n=> n*-1).ToArray();
            var managerEntities = managerIds
                .Select(id => new Employee
                {
                    Id = id,
                    FirstName = Names[randomizer.Next(Names.Length)],
                    LastName = Surnames[randomizer.Next(Surnames.Length)]
                }).ToArray();
            db.Employees.BulkCopy(new BulkCopyOptions{KeepIdentity = true}, managerEntities);

            return db.GetTable<Employee>().ToArray();
        }

        /// <summary>
        /// Selecting from CTE-query employees of requested level 
        /// </summary>
        public static IEnumerable<Employee> GetLeveledEmployees(IQueryable<EmployeeHierarchyCte> hierarchyQuery, int level)
        {
            return hierarchyQuery
                .Where(eh => eh.HierarchyLevel == level)
                .Select(eh => Employee.Build(eh))
                .LoadWith(e => e.EmployeesSubordinate)
                .ToArray();
        }

        /// <summary>
        /// Selecting from CTE-query employees with full loaded associations 
        /// </summary>
        public static IEnumerable<Employee> GetLeveledWithContext(this TestDataConnection db, IQueryable<EmployeeHierarchyCte> hierarchyQuery,
            int level)
        {
            var idQuery = hierarchyQuery
                .Where(eh => eh.HierarchyLevel == level)
                .Select(eh => eh.Id);

            return (
                    from id in idQuery
                    from e in db.Employees.InnerJoin(e => e.Id == id)
                    select e)
                .LoadWith(e => e.Manager)
                .LoadWith(e => e.EmployeesSubordinate)
                .ToArray();
        }
        
        /// <summary>
        /// Generate NewLevel Employees 
        /// </summary>
        public static void GenerateNewLevelEmployees(TestDataConnection db, int maxSubordinates = 4)
        {
            var randomizer = new Random();
            
            var hierarchyQuery = db.GetHierarchyEmployeesCte();
            var maxLevel = GetMaxLevel(hierarchyQuery);
            IEnumerable<Employee> lastLevelEmployees = maxLevel < 1 
                ? db.GenerateRootManagers() 
                : GetLeveledEmployees(hierarchyQuery, maxLevel);

            var newEmployees = lastLevelEmployees
                .Select(
                    newManager => Enumerable.Range(1, randomizer.Next(maxSubordinates)).Select(
                        r => new Employee
                        {
                            FirstName = Names[randomizer.Next(Names.Length)],
                            LastName = Surnames[randomizer.Next(Surnames.Length)],
                            ManagerId = newManager.Id
                        }))
                .SelectMany(e => e);
            db.Employees.BulkCopy(newEmployees);
        }

        /// <summary>
        /// Load end Enumerate Employees with Manager and Subordinate instances specified level
        /// </summary>
        public static void LoadEmployeesOfLevelWithFullAssociations(TestDataConnection db)
        {
            Configuration.Linq.AllowMultipleQuery = true;
                
            var query = db.GetHierarchyEmployeesCte();
            var level = GetMaxLevel(query)-1;
            var employees = db.GetLeveledWithContext(query, level);
            foreach (var employee in employees)
                Console.WriteLine(employee.ToString());
                
            Configuration.Linq.AllowMultipleQuery = false;
        } 
    }
}