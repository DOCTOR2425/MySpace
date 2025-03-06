namespace InstrumentStore.Domain.Contracts.Products
{
	public class ProductPropertyValuesResponse
	{
		public string Name { get; set; } = string.Empty;
		public bool IsRanged { get; set; } = false;
		public string Value { get; set; } = string.Empty;

		public ProductPropertyValuesResponse() { }

		public ProductPropertyValuesResponse(string name, bool isRanged, string value)
		{
			Name = name;
			IsRanged = isRanged;
			Value = value;
		}
	}
}
