namespace InstrumentStore.Domain.Contracts.Products
{
	public class ProductPropertyResponse
	{
		public Guid ProductPropertyId {  get; set; }

        public string Name { get; set; } = string.Empty;
		public bool IsRanged { get; set; } = false;

		public ProductPropertyResponse() { }

        public ProductPropertyResponse(Guid productPropertyId, string name, bool isRanged)
        {
            ProductPropertyId = productPropertyId;
            Name = name;
            IsRanged = isRanged;
        }
    }
}
