namespace InstrumentStore.Domain.DataBase.Models
{
	public class ProductArchive
	{
		public Guid ProductArchiveId { get; set; }
		public string Name { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public decimal Price { get; set; }
		public int Quantity { get; set; }
		public string Brand { get; set; } = string.Empty;
		public string Country { get; set; } = string.Empty;
		public DateTime OrderDate { get; set; }

		public required ProductCategory ProductCategory { get; set; }
		public required User User { get; set; }
	}
}
