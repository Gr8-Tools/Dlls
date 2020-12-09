using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LinqToDB;
using LinqToDB.Mapping;

namespace gRpcLinq2dbServer.DataSource.Models
{
    [Table(Schema = "default")]
    public class Employee
    {
        [PrimaryKey, Identity, Column("EMPLOYEE_ID")]
        public int Id { get; set; }
        
        [Column("FIRST_NAME", Length = 50, CanBeNull = false)]
        public string FirstName { get; set; }
        
        [Column("LAST_NAME", Length = 50, CanBeNull = false)]
        public string LastName { get; set; }
        
        [Column("MANAGER_ID", CanBeNull = true)]
        public int? ManagerId { get; set; }

        #region ASSOSIATIONS

        [Association(
            ThisKey = "MANAGER_ID", OtherKey = "EMPLOYEE_ID", CanBeNull = true,
            QueryExpressionMethod = nameof(ManagerExpression))]
        public Employee? Manager { get; set; }
        
        [Association(
            ThisKey = "EMPLOYEE_ID", OtherKey = "MANAGER_ID", CanBeNull = true,
            QueryExpressionMethod = nameof(SubordinateExpression))]
        public IEnumerable<Employee>? EmployeesSubordinate { get; set; }

        #endregion

        #region EXPRESSIONS

        public static Expression<Func<Employee, IDataContext, IQueryable<Employee>>> ManagerExpression =>
            (e, db) => db.GetTable<Employee>().Where(m => m.Id == e.ManagerId);
        
        public static Expression<Func<Employee, IDataContext, IQueryable<Employee>>> SubordinateExpression =>
            (m, db) => db.GetTable<Employee>().Where(e => e.ManagerId != null && m.Id == e.ManagerId);

        #endregion

        public override string ToString()
        {
            var managerString = ManagerId?.ToString() ?? "NONE";
            var subordinatesString = (EmployeesSubordinate?.Any() ?? false)
                ? string.Join(";", EmployeesSubordinate.Select(e => e.Id))
                : "NONE";
            return $"Employee [{Id}]: {FirstName} {LastName} reports to Manager [{managerString}]. Subordinates: [{subordinatesString}]";
        }
            

        public static Employee Build(EmployeeHierarchyCte eh)
        {
            return new Employee
            {
                Id = eh.Id,
                FirstName = eh.FirstName,
                LastName = eh.LastName,
                ManagerId = eh.ManagerId
            };
        }
    }

    public class EmployeeHierarchyCte
    {
        public int Id;
        public string FirstName;
        public string LastName;
        public int? ManagerId;
        public int HierarchyLevel;

        public override string ToString()
            => $"Employee [{Id}]: {FirstName} {LastName} reports to Manager [{ManagerId?.ToString() ?? "NONE"}]. HierarchyLevel={HierarchyLevel}";
    }
}