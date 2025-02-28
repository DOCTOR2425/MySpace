namespace InstrumentStore.Domain.DataBase.Models
{
	public class Product
	{
		public Guid ProductId { get; set; }
		public string Name { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public decimal Price { get; set; }
		public int Quantity { get; set; }

		public Brand Brand { get; set; } = new Brand();
		public Country Country { get; set; } = new Country();
		public ProductCategory ProductCategory { get; set; } = new ProductCategory();
	}
}
