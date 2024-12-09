using InstrumentStore.Domain.DataBase.Models;

namespace InstrumentStore.Domain.Abstractions
{
    public interface IProductTypeService
    {
        Task<Guid> Create(ProductType brand);
        Task<Guid> Delete(Guid id);
        Task<List<ProductType>> GetAll();
        Task<ProductType> GetById(Guid id);
        Task<Guid> Update(Guid oldId, ProductType newProductType);
    }
}