namespace InstrumentStore.Domain.DataBase.Models
{
    public class ProductProperty
    {
        public Guid ProductPropertyId { get; set; }
        public string Name { get; set; } = string.Empty;

        public required ProductCategory ProductCategory { get; set; }
    }
}
