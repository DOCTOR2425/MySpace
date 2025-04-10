namespace InstrumentStore.Domain.Contracts.ProductProperties
{
    public class ProductPropertyValuesResponse
    {
        public Guid PropertyId { get; set; }
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
