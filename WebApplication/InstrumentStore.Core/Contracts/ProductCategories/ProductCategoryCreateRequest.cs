namespace InstrumentStore.Domain.Contracts.ProductCategories
{
    public class ProductCategoryCreateRequest
    {
        public string Name { get; set; } = string.Empty;
        public Dictionary<string, bool> Properties { get; set; }
    }
}
