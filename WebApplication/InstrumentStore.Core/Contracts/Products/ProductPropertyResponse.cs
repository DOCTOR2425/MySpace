namespace InstrumentStore.Domain.Contracts.Products
{
	public class ProductPropertyResponse
	{
		public string Name { get; set; } = string.Empty;
		public bool IsRanged { get; set; } = false;

		public ProductPropertyResponse() { }

		public ProductPropertyResponse(string name, bool isRanged)
		{
			Name = name;
			IsRanged = isRanged;
		}
	}
}
