using LinqToDB.Mapping;

namespace linq2dbTest.Models
{
    [Table(Schema = "default")]
    public class City
    {
        [PrimaryKey, Identity, Column("CITY_ID")]
        public int Id { get; set; }
        
        [Column("CITY_NAME", Length = 50, CanBeNull = false)]
        public string Name { get; set; }

        public override string ToString()
            => $"City [{Id}]: Name=\"{Name}\"";
    }
}