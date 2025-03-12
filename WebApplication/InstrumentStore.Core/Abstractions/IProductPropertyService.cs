using InstrumentStore.Domain.Contracts.Filters;
using InstrumentStore.Domain.DataBase.Models;

namespace InstrumentStore.Domain.Abstractions
{
	public interface IProductPropertyService
	{
		Task<Guid> CreateProperty(ProductProperty productProperty);
		Task<Guid> CreatePropertyValue(ProductPropertyValue propertyValue);
		Task<Dictionary<string, string>> GetProductProperties(Guid productId);
		Task<CategoryFilters> GetCategoryFilters(string categoryName);
		Task<RangePropertyForFilter[]> GetRangeProperties(Guid categoryId);
		Task<List<ProductPropertyValue>> GetValuesByCategoryName(string categoryName);
		Task<ProductProperty> GetById(Guid id);
		Task DeleteProperiesValuesByProductId(Guid productId);

	}
}