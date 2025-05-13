using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.DataBase;
using InstrumentStore.Domain.DataBase.Models;
using Microsoft.EntityFrameworkCore;

namespace InstrumentStore.Domain.Services
{
	public class ProductComparisonService : IProductComparisonService
	{
		private readonly InstrumentStoreDBContext _dbContext;
		private readonly IProductService _productService;
		private readonly IUserService _usersService;

		public ProductComparisonService(
			InstrumentStoreDBContext dbContext,
			IProductService productService,
			IUserService usersService)
		{
			_dbContext = dbContext;
			_productService = productService;
			_usersService = usersService;
		}

		public async Task<List<ProductComparisonItem>> GetUserComparisonItems(Guid userId)
		{
			return await _dbContext.ProductComparisonItem
				.Include(i => i.User)
				.Include(i => i.Product)
				.Where(i => i.User.UserId == userId)
				.ToListAsync();
		}

		public async Task<List<Product>> GetUserComparisonProducts(Guid userId)
		{
			return await _dbContext.ProductComparisonItem
				.Include(i => i.User)
				.Include(i => i.Product)
				.Where(i => i.User.UserId == userId)
				.Select(i => i.Product)
				.OrderBy(p => p.ProductCategory.Name)
				.ToListAsync();
		}

		public async Task<Guid> AddToComparison(Guid userId, Guid productId)
		{
			ProductComparisonItem? target = await _dbContext.ProductComparisonItem
				.Include(i => i.Product)
				.FirstOrDefaultAsync(i => i.Product.ProductId == productId);

			if (target != null)
				return target.Product.ProductId;

			ProductComparisonItem item = new ProductComparisonItem()
			{
				ProductComparisonItemId = Guid.NewGuid(),
				Product = await _productService.GetById(productId),
				User = await _usersService.GetById(userId),
			};

			await _dbContext.ProductComparisonItem.AddAsync(item);
			await _dbContext.SaveChangesAsync();

			return item.ProductComparisonItemId;
		}

		public async Task DeleteFromComparison(Guid userId, Guid productId)
		{
			await _dbContext.ProductComparisonItem
				.Where(i => i.User.UserId == userId &&
					i.Product.ProductId == productId)
				.ExecuteDeleteAsync();
		}

		public async Task ClearComparisonList(Guid userId)
		{
			await _dbContext.ProductComparisonItem
				.Where(i => i.User.UserId == userId)
				.ExecuteDeleteAsync();
		}

		public async Task<bool> IsProductInUserComparison(Guid productId, Guid userId)
		{
			ProductComparisonItem? target = await _dbContext.ProductComparisonItem
				.FirstOrDefaultAsync(i => i.User.UserId == userId &&
					i.Product.ProductId == productId);

			if (target == null)
				return false;
			return true;
		}
	}
}
