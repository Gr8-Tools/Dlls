using System;
using System.Linq;
using System.Linq.Expressions;
using LinqToDB;
using LinqToDB.Mapping;

namespace linq2dbTest.Models
{
    [Table(Schema = "default")]
    public class Supplier
    {
        [PrimaryKey, Identity, Column("SUPPLIER_ID")]
        public int Id { get; set; }

        [Column("SUPPLIER_NAME", Length = 50, CanBeNull = false)]
        public string Name { get; set; }
        
        [Column("CITY_ID", CanBeNull = false)]
        public int CityId { get; set; }

        #region ASSOSIATIONS

        [Association(ThisKey = "CITY_ID", OtherKey = "CITY_ID", CanBeNull = false,
                QueryExpressionMethod = nameof(SupplierCityExpression))]
        public City? SuppliersCity { get; set; }
        
        #endregion

        #region EXPRESSIONS

        public static Expression<Func<Supplier, IDataContext, IQueryable<City>>> SupplierCityExpression =>
            (s, db) => db.GetTable<City>().Where(c => c.Id == s.CityId);

        #endregion

        public override string ToString()
        {
            var cityValue = SuppliersCity?.ToString() ?? CityId.ToString();
            return $"Supplier [{Id}]: Name=\"{Name}\";City=\"{cityValue}\";";
        }
    }
}