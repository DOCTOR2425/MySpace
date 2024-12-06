namespace InstrumentStore.Domain.DataBase.Models
{
    public class Product
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public byte[] Image { get; set; }

        public required ProductType ProductType { get; set; }
        public required Brand Brand { get; set; }
        public required Country Country { get; set; }
    }
}
