namespace InstrumentStore.Domain.DataBase.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public byte[] Image { get; set; }

        public ProductType ProductType { get; set; }
        public Country Country { get; set; }
        public Brand Brand { get; set; }
    }
}
