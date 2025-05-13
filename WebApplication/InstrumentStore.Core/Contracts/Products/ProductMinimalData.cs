namespace InstrumentStore.Domain.Contracts.Products
{
	public class ProductMinimalData
	{
		public Guid ProductId { get; set; }
		public string Name { get; set; } = string.Empty;
		public decimal Price { get; set; }
		public string Image { get; set; } = string.Empty;
		public bool IsArchive { get; set; } = false;
	}
}
