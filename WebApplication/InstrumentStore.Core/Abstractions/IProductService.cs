using InstrumentStore.Domain.Contracts.Filters;
using InstrumentStore.Domain.Contracts.Products;
using InstrumentStore.Domain.DataBase.Models;

namespace InstrumentStore.Domain.Abstractions
{
    public interface IProductService
    {
        Task<Guid> Create(Product product);
        Task<Guid> Create(ProductRequest productRequest);
        Task<Guid> Delete(Guid id);
        Task<List<Product>> GetAll();
        Task<Product> GetById(Guid id);
        Task<Guid> Update(Guid oldId, Product newProduct);
        Task<Guid> Update(Guid oldId, ProductRequest newProduct);
        Task<List<Product>> GetAllByCategory(string categoryName);
        Task<List<Product>> GetAllWithFilters(FilterRequest filter, List<Product> productsForFilter);
    }
}