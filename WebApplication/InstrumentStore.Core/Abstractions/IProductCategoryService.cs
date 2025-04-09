using InstrumentStore.Domain.Contracts.Some;
using InstrumentStore.Domain.DataBase.Models;

namespace InstrumentStore.Domain.Abstractions
{
    public interface IProductCategoryService
    {
        Task<Guid> Create(ProductCategory productCategory);
        Task<List<ProductCategory>> GetAll();
        Task<ProductCategory> GetById(Guid id);
        Task<Guid> Update(Guid oldId, ProductCategory newProductType);
        Task<List<ProductProperty>> GetProductPropertiesByCategory(string category);
        Task<List<ProductProperty>> GetProductPropertiesByCategory(Guid categoryId);
        Task<List<ProductCategory>> GetCategoriesBySales();
        Task<List<ProductCategoryForAdmin>> GetCategoriesForAdmin();
        Task<Guid> ChangeVisibilityStatus(Guid categoryId, bool visibilityStatus);

    }
}