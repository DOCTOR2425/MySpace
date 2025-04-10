using InstrumentStore.Domain.Contracts.ProductProperties;

namespace InstrumentStore.Domain.Contracts.Products
{
    public class FullProductInfoResponse
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string ProductCategory { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public bool IsArchive { get; set; }

        public List<string> Images { get; set; }
        public List<ProductPropertyValuesResponse> ProductPropertyValues { get; set; }

        public FullProductInfoResponse()
        {
            Images = new List<string>();
            ProductPropertyValues = new List<ProductPropertyValuesResponse>();
        }

        public FullProductInfoResponse(Guid productId,
            string name,
            string description,
            decimal price,
            int quantity,
            string productCategory,
            string brand,
            string country,
            List<string> images,
            List<ProductPropertyValuesResponse> productPropertyValues)
        {
            ProductId = productId;
            Name = name;
            Description = description;
            Price = price;
            Quantity = quantity;
            ProductCategory = productCategory;
            Brand = brand;
            Country = country;
            Images = images;
            ProductPropertyValues = productPropertyValues;
        }
    }
}
