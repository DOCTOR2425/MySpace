using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.DataBase;
using InstrumentStore.Domain.DataBase.Models;
using Microsoft.EntityFrameworkCore;

namespace InstrumentStore.Domain.Services
{
    public class ProductPropertyService : IProductPropertyService
    {
        private readonly InstrumentStoreDBContext _dbContext;
        private readonly IBrandService _brandService;
        private readonly ICountryService _countryService;
        private readonly IProductCategoryService _productCategoryService;
        private readonly IProductService _productService;

        public ProductPropertyService(InstrumentStoreDBContext dbContext,
            IBrandService brandService,
            ICountryService countryService,
            IProductCategoryService productCategoryService,
            IProductService productService)
        {
            _dbContext = dbContext;
            _brandService = brandService;
            _countryService = countryService;
            _productCategoryService = productCategoryService;
            _productService = productService;
        }

        public async Task<Guid> CreateProperty(ProductProperty productProperty)
        {
            await _dbContext.ProductProperty.AddAsync(productProperty);
            await _dbContext.SaveChangesAsync();

            return productProperty.ProductPropertyId;
        }

        public async Task<Guid> CreatePropertyValue(ProductPropertyValue propertyValue)
        {
            await _dbContext.ProductPropertyValue.AddAsync(propertyValue);
            await _dbContext.SaveChangesAsync();

            return propertyValue.ProductPropertyValueId;
        }

        public async Task<Dictionary<string, string>> GetProductProperties(Guid productId)
        {
            Dictionary<string, string> properties = new Dictionary<string, string>();

            ProductPropertyValue[] productPropertyValues =
                await _dbContext.ProductPropertyValue
                    .Include(prop => prop.Product)
                    .Include(prop => prop.ProductProperty)
                    .ToArrayAsync();

            foreach (var propertyValue in productPropertyValues)
            {
                if (propertyValue.Product.ProductId == productId)
                {
                    properties.Add(propertyValue.ProductProperty.Name, propertyValue.Value);
                }
            }

            return properties;
        }
    }
}
