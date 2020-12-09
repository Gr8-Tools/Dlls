using LinqToDB.Mapping;

namespace gRpcLinq2dbServer.DataSource.Models
{
    [Table("Products_suppliers", Schema = "default")]
    public class ProductsSuppliers
    {
        // [PrimaryKey, Identity, Column("PRODUCT_SUPPLIER_IDENTITY")]
        // public string Identity { get; set; }

        [Column("PRODUCT_ID", CanBeNull = false)]
        public int ProductId { get; set; }
        
        [Column("SUPPLIER_ID", CanBeNull = false)]
        public int SupplierId { get; set; }

        // public static string IdentityGenerator(int productId, int supplierId)
        //     => $"{productId} : {supplierId}";
        
        public override string ToString()
            => $"Product[{ProductId}] <-> Supplier[{SupplierId}]";
    }
}