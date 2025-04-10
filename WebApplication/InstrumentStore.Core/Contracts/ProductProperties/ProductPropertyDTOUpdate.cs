namespace InstrumentStore.Domain.Contracts.ProductProperties
{
    public class ProductPropertyDTOUpdate
    {
        public Guid? ProductPropertyId { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsRanged { get; set; } = false;
        public string? DefaultValue { get; set; }

        public ProductPropertyDTOUpdate() { }

        public ProductPropertyDTOUpdate(Guid productPropertyId, string name, bool isRanged)
        {
            ProductPropertyId = productPropertyId;
            Name = name;
            IsRanged = isRanged;
        }

    }
}
