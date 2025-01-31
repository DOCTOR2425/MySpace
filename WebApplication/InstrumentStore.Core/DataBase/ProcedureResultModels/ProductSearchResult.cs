namespace InstrumentStore.Domain.DataBase.ProcedureResultModels
{
    public class ProductSearchResult
    {
        public Guid ProductId { get; set; }
        public Guid BrandId { get; set; }
        public Guid CountryId { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public Guid ProductCategoryId { get; set; }
        public int Quantity { get; set; }

        public Guid ProductCategoryId2 { get; set; }
        public string ProductCategoryName { get; set; } = string.Empty;
        public Guid BrandId2 { get; set; }
        public string BrandName { get; set; } = string.Empty;
        public Guid CountryId2 { get; set; }
        public string CountryName { get; set; } = string.Empty;
    }
}
