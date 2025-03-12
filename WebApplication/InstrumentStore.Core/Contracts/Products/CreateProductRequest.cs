namespace InstrumentStore.Domain.Contracts.Products
{
	public class CreateProductRequest
	{
		public string Name { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public decimal Price { get; set; }
		public int Quantity { get; set; }
		public Dictionary<Guid, string> PropertyValues { get; set; }
		public Guid ProductCategoryId { get; set; }
		public Guid BrandId { get; set; }
		public Guid CountryId { get; set; }
	}
}
