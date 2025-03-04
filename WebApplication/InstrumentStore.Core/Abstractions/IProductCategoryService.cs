using InstrumentStore.Domain.DataBase.Models;

namespace InstrumentStore.Domain.Abstractions
{
	public interface IProductCategoryService
	{
		Task<Guid> Create(ProductCategory brand);
		Task<Guid> Delete(Guid id);
		Task<List<ProductCategory>> GetAll();
		Task<ProductCategory> GetById(Guid id);
		Task<Guid> Update(Guid oldId, ProductCategory newProductType);
		Task<List<ProductProperty>> GetProductPropertiesByCategoryId(Guid categoryId);

	}
}