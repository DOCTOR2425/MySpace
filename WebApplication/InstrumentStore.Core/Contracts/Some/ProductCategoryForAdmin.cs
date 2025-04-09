namespace InstrumentStore.Domain.Contracts.Some
{
    public class ProductCategoryForAdmin
    {
        public Guid ProductCategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int ProductCount { get; set; } = 0;
        public bool IsHidden { get; set; } = false;
    }
}
