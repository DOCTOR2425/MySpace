namespace InstrumentStore.Domain.Contracts.Products
{
	public class AdminProductCard
	{
		public Guid ProductId { get; set; }
		public string Name { get; set; }
		public decimal Price { get; set; }
		public int Quantity { get; set; }
		public string Image { get; set; }
		public string ProductCategory { get; set; }
		public string Brand { get; set; }
		public string Country { get; set; }
		public bool IsArchive { get; set; } = false;

		public AdminProductCard() { }
	}
}
