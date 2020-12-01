namespace linq2dbTest.Models.Examples
{
    public class JoinProduct
    {
        public int Id { get; set; }
        
        public string Name { get; set; }
        
        public string CategoryName { get; set; }

        public override string ToString()
            => $"LeftJoinResult: \"ID\"=[{Id}]; \"Name\"=\"{Name}\"; \"CategoryName\"=\"{CategoryName}\";";
        
        public static JoinProduct Build(Product? product, Category category)
        {
            return new JoinProduct
            {
                Id = product?.Id ?? -1,
                Name = product?.Name ?? "",
                CategoryName = category.Name
            };
        }
    }
}