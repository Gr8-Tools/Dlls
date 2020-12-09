using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LinqToDB;
using LinqToDB.Mapping;

namespace gRpcLinq2dbServer.DataSource.Models
{
    [Table(Schema = "default")]
    public class Category
    {
        [PrimaryKey, Identity, Column(Name = "CATEGORY_ID")]
        public int Id { get; set; }

        [Column(Name = "CATEGORY_NAME", Length = 50), NotNull] 
        public string Name { get; set; }

        #region ASSOSIATIONS

        /// <summary>
        /// Product_Category_BackReference
        /// </summary>
        [Association(
            ThisKey = "CATEGORY_ID", OtherKey = "CATEGORY_ID", CanBeNull = false, 
            QueryExpressionMethod = nameof(ToProductsExpression))]
        public IEnumerable<Product> ProductCategoryIdfKeys { get; set; }

        #endregion
        
        #region EXPRESSIONS

        public static Expression<Func<Category, IDataContext, IQueryable<Product>>> ToProductsExpression =>
            (c, db) => db.GetTable<Product>().Where(p => p.CategoryId == c.Id);

        #endregion
        
        public override string ToString()
            => $"Category [{Id}]: \"Name\"=\"{Name}\";";
    }
}