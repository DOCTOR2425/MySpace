using InstrumentStore.Domain.Contracts.ProductProperties;

namespace InstrumentStore.Domain.Contracts.ProductCategories
{
    public class ProductCategoryDTOUpdate
    {
        public string Name { get; set; } = string.Empty;
        public List<ProductPropertyDTOUpdate> Properties { get; set; }
    }
}
