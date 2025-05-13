namespace InstrumentStore.Domain.Contracts.Products
{
	public class UserProductCard
	{
		public Guid ProductId { get; set; }
		public string Name { get; set; }
		public decimal Price { get; set; }
		public string Image { get; set; }
		public bool IsArchive { get; set; } = false;
		public int CartCount { get; set; }

		public UserProductCard() { }
	}
}
