using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.Contracts.Filters;
using InstrumentStore.Domain.DataBase;
using InstrumentStore.Domain.DataBase.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

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

        public async Task<CategoryFilters> GetCategoryFilters(string categoryName)
        {
            Guid categoryId = (await _dbContext.ProductCategory
                .FirstOrDefaultAsync(c => c.Name.ToLower() == categoryName.ToLower()))
                .ProductCategoryId;

            RangePropertyForFilter[] rangePropertyForFilters = await GetRangeProperties(categoryId);
            CollectionPropertyForFilter[] collectionPropertyForFilters = await GetCollectionProperties(categoryId);

            CategoryFilters categoryFilters = new CategoryFilters(
                rangePropertyForFilters,
                collectionPropertyForFilters);

            return categoryFilters;
        }

        private async Task<CollectionPropertyForFilter[]> GetCollectionProperties(Guid categoryId)
        {
            List<CollectionPropertyForFilter> collectionPropertyForFilter
                = new List<CollectionPropertyForFilter>();
            List<ProductProperty> productProperty = await _dbContext.ProductProperty
                .Where(p => p.ProductCategory.ProductCategoryId == categoryId &&
                    p.IsRanged == false)
                .ToListAsync();

            foreach (var property in productProperty)
            {
                string[] uniqueValues = await _dbContext.ProductPropertyValue
                    .Where(pv => 
                        pv.ProductProperty.ProductCategory.ProductCategoryId == categoryId &&
                        pv.ProductProperty.Name == property.Name)
                    .Select(pv => pv.Value)
                    .Distinct()
                    .ToArrayAsync();

                collectionPropertyForFilter.Add(new CollectionPropertyForFilter(
                    property.Name,
                    uniqueValues));
            }

            return collectionPropertyForFilter.ToArray();
        }

        public async Task<RangePropertyForFilter[]> GetRangeProperties(Guid categoryId)
        {
            List<RangePropertyForFilter> rangePropertyForFilters = new List<RangePropertyForFilter>();
            List<ProductProperty> productProperty = await _dbContext.ProductProperty
                .Where(p => p.ProductCategory.ProductCategoryId == categoryId &&
                    p.IsRanged)
                .ToListAsync();

            foreach (var property in productProperty)
            {
                List<ProductPropertyValue> targetPropertyValues = await _dbContext.ProductPropertyValue
                    .Where(pv => pv.ProductProperty.ProductPropertyId == property.ProductPropertyId)
                    .ToListAsync();

                rangePropertyForFilters.Add(new RangePropertyForFilter(
                    property.Name,
                    targetPropertyValues.Max(pv => decimal.Parse(pv.Value)),
                    targetPropertyValues.Min(pv => decimal.Parse(pv.Value))));
            }

            return rangePropertyForFilters.ToArray();
        }
    }
}
