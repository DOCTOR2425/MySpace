namespace InstrumentStore.Domain.Contracts.Products
{
	public class ProductPropertyResponse
	{
		public string PropertyName { get; set; } = string.Empty;
		public bool IsRanged { get; set; } = false;
	}
}
