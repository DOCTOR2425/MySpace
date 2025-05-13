using InstrumentStore.Domain.DataBase.Models;

namespace InstrumentStore.Domain.Abstractions
{
	public interface IProductComparisonService
	{
		Task<Guid> AddToComparison(Guid userId, Guid productId);
		Task DeleteFromComparison(Guid userId, Guid productId);
		Task<List<ProductComparisonItem>> GetUserComparisonItems(Guid userId);
		Task<List<Product>> GetUserComparisonProducts(Guid userId);
		Task ClearComparisonList(Guid userId);
		Task<bool> IsProductInUserComparison(Guid productId, Guid userId);

	}
}