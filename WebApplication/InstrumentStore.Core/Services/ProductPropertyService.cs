using System.Globalization;
using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.Contracts.Filters;
using InstrumentStore.Domain.DataBase;
using InstrumentStore.Domain.DataBase.Models;
using Microsoft.EntityFrameworkCore;

namespace InstrumentStore.Domain.Services
{
    public class ProductPropertyService : IProductPropertyService
    {
        private readonly InstrumentStoreDBContext _dbContext;

        public ProductPropertyService(InstrumentStoreDBContext dbContext)
        {
            _dbContext = dbContext;
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

        public async Task<ProductProperty> GetById(Guid id)
        {
            return await _dbContext.ProductProperty
                .Include(p => p.ProductCategory)
                .FirstOrDefaultAsync(p => p.ProductPropertyId == id);
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
            try
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
            catch (NullReferenceException ex)
            {
                throw new NullReferenceException("Нет ещё такой категории");
            }
        }

        private async Task<CollectionPropertyForFilter[]> GetCollectionProperties(Guid categoryId)
        {
            List<CollectionPropertyForFilter> collectionPropertyForFilter
                = new List<CollectionPropertyForFilter>();
            collectionPropertyForFilter.AddRange(await GetStaticsPropertiesToCollectionProperties(categoryId));

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

        private async Task<List<CollectionPropertyForFilter>> GetStaticsPropertiesToCollectionProperties(
            Guid categoryId)
        {
            List<CollectionPropertyForFilter> collectionPropertyForFilter
                = new List<CollectionPropertyForFilter>();

            string[] uniqueValues = await _dbContext.Product
                    .Where(p => p.ProductCategory.ProductCategoryId == categoryId)
                    .Select(p => p.Brand.Name)
                    .Distinct()
                    .ToArrayAsync();

            collectionPropertyForFilter.Insert(0, new CollectionPropertyForFilter(
                "Бренд",
                uniqueValues));

            return collectionPropertyForFilter;
        }

        public async Task<RangePropertyForFilter[]> GetRangeProperties(Guid categoryId)
        {
            List<RangePropertyForFilter> rangePropertyForFilters = [.. await GetStaticsPropertiesToRangeProperties(categoryId)];
            List<ProductProperty> productProperty = await _dbContext.ProductProperty
                .Where(p => p.ProductCategory.ProductCategoryId == categoryId &&
                    p.IsRanged)
                .ToListAsync();

            NumberFormatInfo formatInfo = new NumberFormatInfo()
            {
                NumberDecimalSeparator = "."
            };

            foreach (var property in productProperty)
            {
                List<ProductPropertyValue> targetPropertyValues = await _dbContext.ProductPropertyValue
                    .Where(pv => pv.ProductProperty.ProductPropertyId == property.ProductPropertyId)
                    .ToListAsync();

                rangePropertyForFilters.Add(new RangePropertyForFilter(
                    property.Name,
                    Math.Ceiling(targetPropertyValues.Max(pv => decimal.Parse(pv.Value, formatInfo))),
                    Math.Floor(targetPropertyValues.Min(pv => decimal.Parse(pv.Value, formatInfo)))));
            }
            return rangePropertyForFilters.ToArray();
        }

        private async Task<List<RangePropertyForFilter>> GetStaticsPropertiesToRangeProperties(
            Guid categoryId)
        {
            List<RangePropertyForFilter> rangePropertyForFilters
                = new List<RangePropertyForFilter>();

            List<decimal> productPrices = await _dbContext.Product
                    .Where(p => p.ProductCategory.ProductCategoryId == categoryId)
                    .Select(p => p.Price).ToListAsync();

            rangePropertyForFilters.Insert(0, new RangePropertyForFilter(
                "Цена",
                Math.Ceiling(productPrices.Max()),
                Math.Floor(productPrices.Min())));

            return rangePropertyForFilters;
        }

        public async Task<List<ProductPropertyValue>> GetValuesByCategoryName(string categoryName)
        {
            return await _dbContext.ProductPropertyValue
                .Where(v => v.ProductProperty.ProductCategory.Name == categoryName)
                .Include(v => v.Product)
                .Include(v => v.ProductProperty)
                .ToListAsync();
        }

        public async Task DeleteProperiesValuesByProductId(Guid productId)
        {
            await _dbContext.ProductPropertyValue
                .Where(p => p.Product.ProductId == productId)
                .ExecuteDeleteAsync();
        }

        public async Task<Guid> DeleteById(Guid propertyId)
        {
            await _dbContext.ProductProperty
                .Where(p => p.ProductPropertyId == propertyId)
                .ExecuteDeleteAsync();

            return propertyId;
        }
    }
}
