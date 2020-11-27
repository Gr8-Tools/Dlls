namespace linq2dbTest.Models.Examples
{
    public class LeftJoinProduct
    {
        public int Id { get; set; }
        
        public string Name { get; set; }
        
        public string CategoryName { get; set; }

        public override string ToString()
            => $"LeftJoinResult: \"ID\"=[{Id}]; \"Name\"=\"{Name}\"; \"CategoryName\"=\"{CategoryName}\";";
        
        public static LeftJoinProduct Build(Product product, Category category)
        {
            return new LeftJoinProduct
            {
                Id = product.Id,
                Name = product.Name,
                CategoryName = category.Name
            };
        }
    }
}