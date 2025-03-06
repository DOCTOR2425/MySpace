using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.DataBase;
using InstrumentStore.Domain.DataBase.Models;
using Microsoft.EntityFrameworkCore;

namespace InstrumentStore.Domain.Services
{
	public class ProduCtategoryService : IProductCategoryService
	{
		private readonly InstrumentStoreDBContext _dbContext;

		public ProduCtategoryService(InstrumentStoreDBContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<List<ProductCategory>> GetAll()
		{
			return await _dbContext.ProductCategory.AsNoTracking().ToListAsync();
		}

		public async Task<ProductCategory> GetById(Guid id)
		{
			return await _dbContext.ProductCategory.FirstOrDefaultAsync(x => x.ProductCategoryId == id);
		}

		public async Task<Guid> Create(ProductCategory brand)
		{
			await _dbContext.ProductCategory.AddAsync(brand);
			await _dbContext.SaveChangesAsync();

			return brand.ProductCategoryId;
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

		public async Task<Guid> Delete(Guid id)
		{
			await _dbContext.ProductCategory
				.Where(p => p.ProductCategoryId == id)
				.ExecuteDeleteAsync();

			return id;
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
	}
}
