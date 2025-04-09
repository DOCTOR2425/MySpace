using AutoMapper;
using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.Contracts.Some;
using InstrumentStore.Domain.DataBase;
using InstrumentStore.Domain.DataBase.Models;
using Microsoft.EntityFrameworkCore;

namespace InstrumentStore.Domain.Services
{
    public class ProduCtategoryService : IProductCategoryService
    {
        private readonly InstrumentStoreDBContext _dbContext;
        private readonly IMapper _mapper;

        public ProduCtategoryService(InstrumentStoreDBContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<List<ProductCategory>> GetAll()
        {
            List<ProductCategory> topCategories = await _dbContext.PaidOrderItem
                .Include(i => i.Product.ProductCategory)
                .GroupBy(i => i.Product.ProductCategory)
                .Select(g => new
                {
                    Category = g.Key,
                    TotalSales = g.Sum(i => i.Price * i.Quantity)
                })
                .OrderByDescending(x => x.TotalSales)
                .Select(x => x.Category)
                .ToListAsync();

            return topCategories;
        }

        public async Task<ProductCategory> GetById(Guid id)
        {
            return await _dbContext.ProductCategory.FindAsync(id);
        }

        public async Task<Guid> Create(ProductCategory productCategory)
        {
            ProductCategory? category = await _dbContext.ProductCategory
                .FirstOrDefaultAsync(c => c.Name == productCategory.Name);

            if (category == null)
                throw new InvalidOperationException("Такая категория уже существует");

            await _dbContext.ProductCategory.AddAsync(productCategory);
            await _dbContext.SaveChangesAsync();

            return productCategory.ProductCategoryId;
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
