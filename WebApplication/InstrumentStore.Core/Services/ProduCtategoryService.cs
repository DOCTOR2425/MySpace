using AutoMapper;
using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.Contracts.ProductCategories;
using InstrumentStore.Domain.Contracts.ProductProperties;
using InstrumentStore.Domain.DataBase;
using InstrumentStore.Domain.DataBase.Models;
using Microsoft.EntityFrameworkCore;

namespace InstrumentStore.Domain.Services
{
    public class ProduCtategoryService : IProductCategoryService
    {
        private readonly InstrumentStoreDBContext _dbContext;
        private readonly IProductPropertyService _productPropertyService;
        private readonly IMapper _mapper;

        public ProduCtategoryService(
            InstrumentStoreDBContext dbContext,
            IMapper mapper,
            IProductPropertyService productPropertyService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _productPropertyService = productPropertyService;
        }

        public async Task<List<ProductCategory>> GetAll()
        {
            var categoriesWithSales = await _dbContext.ProductCategory
                .GroupJoin(
                    _dbContext.PaidOrderItem.Include(i => i.Product.ProductCategory),
                    category => category.ProductCategoryId,
                    orderItem => orderItem.Product.ProductCategory.ProductCategoryId,
                    (category, orderItems) => new
                    {
                        Category = category,
                        TotalSales = orderItems.Sum(i => i.Price * i.Quantity)
                    })
                .OrderByDescending(x => x.TotalSales)
                .Select(x => x.Category)
                .ToListAsync();

            return categoriesWithSales;
        }


        public async Task<ProductCategory> GetById(Guid id)
        {
            return await _dbContext.ProductCategory.FindAsync(id);
        }

        public async Task<ProductCategoryDTOUpdate> GetProductCategoryResponseById(Guid categoryId)
        {
            ProductCategory productCategory = await GetById(categoryId);
            List<ProductPropertyDTOUpdate> properties = _mapper
                .Map<List<ProductPropertyDTOUpdate>>(await GetProductPropertiesByCategory(categoryId));

            ProductCategoryDTOUpdate categoryDTO = new ProductCategoryDTOUpdate()
            {
                Name = productCategory.Name,
                Properties = properties
            };

            return categoryDTO;
        }

        private async Task CheckCategoryExistence(string categoryName)
        {
            ProductCategory? category = await _dbContext.ProductCategory
                .FirstOrDefaultAsync(c => c.Name == categoryName);

            if (category != null)
                throw new InvalidOperationException("Такая категория уже существует");
        }

        public async Task<Guid> Create(ProductCategory productCategory)
        {
            await CheckCategoryExistence(productCategory.Name);

            await _dbContext.ProductCategory.AddAsync(productCategory);
            await _dbContext.SaveChangesAsync();

            return productCategory.ProductCategoryId;
        }

        public async Task<Guid> Create(ProductCategoryCreateRequest productCategory)
        {
            await CheckCategoryExistence(productCategory.Name);

            ProductCategory category = new ProductCategory()
            {
                ProductCategoryId = Guid.NewGuid(),
                Name = productCategory.Name,
            };
            await Create(category);

            foreach (var property in productCategory.Properties)
                await _productPropertyService.CreateProperty(new ProductProperty()
                {
                    ProductPropertyId = Guid.NewGuid(),
                    Name = property.Key,
                    IsRanged = property.Value,
                    ProductCategory = category,
                });

            return category.ProductCategoryId;
        }

        public async Task<Guid> Update(Guid oldId, ProductCategory newProductType)
        {
            await _dbContext.ProductCategory
                .Where(p => p.ProductCategoryId == oldId)
                .ExecuteUpdateAsync(x => x
                    .SetProperty(p => p.Name, newProductType.Name));

            _dbContext.SaveChanges();

            return oldId;
        }

        public async Task<Guid> Update(Guid categoryId, ProductCategoryDTOUpdate newCategory)
        {
            ProductCategory category = await GetById(categoryId);
            category.Name = newCategory.Name;

            List<ProductProperty> oldProperties = await GetProductPropertiesByCategory(categoryId);
            foreach (var property in oldProperties)
            {
                ProductPropertyDTOUpdate? propertyResponse = newCategory.Properties
                    .Find(p => p.ProductPropertyId == property.ProductPropertyId);

                if (propertyResponse == null)
                {
                    await _productPropertyService.DeleteById(property.ProductPropertyId);
                }
                else
                {
                    property.Name = propertyResponse.Name;
                    property.IsRanged = propertyResponse.IsRanged;

                    if (propertyResponse.DefaultValue != null)
                        await _dbContext.ProductPropertyValue
                            .Where(p => p.ProductProperty.ProductPropertyId == property.ProductPropertyId)
                            .ExecuteUpdateAsync(x => x.SetProperty(p => p.Value, propertyResponse.DefaultValue));
                }
            }

            await CreateNewProperties(category, newCategory.Properties, oldProperties);

            await _dbContext.SaveChangesAsync();
            return categoryId;
        }

        private async Task CreateNewProperties(
            ProductCategory category,
            List<ProductPropertyDTOUpdate> newPropertiesRequest,
            List<ProductProperty> oldProperties)
        {
            List<Product> products = await _dbContext.Product
                .Where(p => p.ProductCategory.ProductCategoryId == category.ProductCategoryId)
                .ToListAsync();

            foreach (var property in newPropertiesRequest)
            {
                ProductProperty? target = oldProperties
                    .Find(p => p.ProductPropertyId == property.ProductPropertyId);

                if (target == null)
                {
                    ProductProperty newProperty = new ProductProperty()
                    {
                        ProductPropertyId = Guid.NewGuid(),
                        Name = property.Name,
                        IsRanged = property.IsRanged,
                        ProductCategory = category
                    };
                    await _productPropertyService.CreateProperty(newProperty);

                    foreach (var product in products)
                    {
                        await _productPropertyService.CreatePropertyValue(new ProductPropertyValue()
                        {
                            ProductPropertyValueId = Guid.NewGuid(),
                            Value = property.DefaultValue,
                            Product = product,
                            ProductProperty = newProperty
                        });
                    }
                }
            }
        }

        public async Task<List<ProductProperty>> GetProductPropertiesByCategory(string category)
        {
            return await _dbContext.ProductProperty
                .Include(p => p.ProductCategory)
                .Where(p => p.ProductCategory.Name == category)
                .ToListAsync();
        }

        public async Task<List<ProductProperty>> GetProductPropertiesByCategory(Guid categoryId)
        {
            return await _dbContext.ProductProperty
                .Include(p => p.ProductCategory)
                .Where(p => p.ProductCategory.ProductCategoryId == categoryId)
                .ToListAsync();
        }

        public async Task<List<ProductCategory>> GetCategoriesBySales()
        {
            List<ProductCategory> categories = await GetAll();
            List<ProductCategory> result = new List<ProductCategory>(categories.Count);

            foreach (var category in categories)
                if (await IsCategoryHidden(category.ProductCategoryId) == false)
                    result.Add(category);

            return result;
        }

        public async Task<List<ProductCategoryForAdmin>> GetCategoriesForAdmin()
        {
            List<ProductCategory> categories = await _dbContext.ProductCategory.ToListAsync();

            var productCounts = await _dbContext.Product
                .Include(p => p.ProductCategory)
                .GroupBy(p => p.ProductCategory.ProductCategoryId)
                .Select(g => new
                {
                    CategoryId = g.Key,
                    Count = g.Count()
                })
                .ToDictionaryAsync(x => x.CategoryId, x => x.Count);

            List<ProductCategoryForAdmin> response = _mapper
                .Map<List<ProductCategoryForAdmin>>(categories);

            foreach (var item in response)
            {
                item.ProductCount = productCounts
                    .TryGetValue(item.ProductCategoryId, out var count) ? count : 0;
                item.IsHidden = await IsCategoryHidden(item.ProductCategoryId);
            }

            return response;
        }

        private async Task<bool> IsCategoryHidden(Guid categoryId)
        {
            return !(await _dbContext.Product
                .Include(p => p.ProductCategory)
                .AnyAsync(p => p.ProductCategory.ProductCategoryId == categoryId && !p.IsArchive));
        }

        public async Task<Guid> ChangeVisibilityStatus(Guid categoryId, bool visibilityStatus)
        {
            await _dbContext.Product
                .Include(p => p.ProductCategory)
                .Where(p => p.ProductCategory.ProductCategoryId == categoryId)
                .ExecuteUpdateAsync(p => p.SetProperty(p => p.IsArchive, visibilityStatus));

            return categoryId;
        }
    }
}
