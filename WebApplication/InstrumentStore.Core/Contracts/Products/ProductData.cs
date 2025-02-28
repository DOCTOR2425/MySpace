namespace InstrumentStore.Domain.Contracts.Products
{
	public class ProductData
	{
		public Guid ProductId { get; set; }
		public string Name { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public decimal Price { get; set; }
		public int Quantity { get; set; }
		public string Image { get; set; } = string.Empty;
		public string ProductCategory { get; set; } = string.Empty;
		public string Brand { get; set; } = string.Empty;
		public string Country { get; set; } = string.Empty;
	}
}
