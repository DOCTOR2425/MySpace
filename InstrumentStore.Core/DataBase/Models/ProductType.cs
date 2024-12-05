namespace InstrumentStore.Domain.DataBase.Models
{
    public class ProductType
    {
        public Guid ProductTypeId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
