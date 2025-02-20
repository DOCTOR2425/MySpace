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
        Task<List<Product>> GetAll(int page);
        Task<Product> GetById(Guid id);
        Task<Guid> Update(Guid oldId, Product newProduct);
        Task<Guid> Update(Guid oldId, ProductRequest newProduct);
        Task<List<Product>> GetAllByCategory(string categoryName, int page);
        Task<List<Product>> GetAllWithFilters(
            string categoryName,
            FilterRequest filter,
            List<Product> productsForFilter, 
            int page);
        Task<List<Product>> SearchByName(string input, int package);
    }
}