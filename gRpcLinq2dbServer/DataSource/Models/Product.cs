using System;
using System.Linq;
using System.Linq.Expressions;
using LinqToDB;
using LinqToDB.Mapping;

namespace gRpcLinq2dbServer.DataSource.Models
{
    [Table(Schema = "default")]
    public class Product
    {
        [PrimaryKey, Identity, Column(Name = "PRODUCT_ID")]
        public int Id { get; set; }
        
        [Column(Name = "PRODUCT_NAME", Length = 50), NotNull]
        public string Name { get; set; }
        
        [Column(Name = "CATEGORY_ID"), NotNull]
        public int CategoryId { get; set; }
        
        #region ASSOSIATIONS

        /// <summary>
        /// Product_Catecory_Reference
        /// </summary>
        [Association(ThisKey = "CATEGORY_ID", OtherKey = "CATEGORY_ID" ,CanBeNull = false, 
            QueryExpressionMethod = nameof(CategoryExpression))]
        public Category CategoryIdfKey { get; set; }

        #endregion

        #region EXPRESSIONS

        public static Expression<Func<Product, IDataContext, IQueryable<Category>>> CategoryExpression =>
            (p, db) => db.GetTable<Category>().Where(c => c.Id == p.CategoryId);

        #endregion

        public override string ToString()
            => $"Product [{Id}]: \"Name\"=\"{Name}\";\"CategoryId\"=[{CategoryId}];";
    }
}