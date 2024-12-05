using InstrumentStore.Domain.DataBase.Models;

namespace InstrumentStore.Domain.Abstractions
{
    public interface IProductService
    {
        Task<Guid> Create(Product product);
        Task<Guid> Delete(Guid id);
        Task<List<Product>> GetAll();
        Task<Product> GetById(Guid id);
        Task<Guid> Update(Guid oldId, Product newProduct);
    }
}