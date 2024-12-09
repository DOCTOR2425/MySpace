using InstrumentStore.Domain.DataBase.Models;

namespace InstrumentStore.Domain.Abstractions
{
    public interface IBrandService
    {
        Task<Guid> Create(Brand brand);
        Task<Guid> Delete(Guid id);
        Task<List<Brand>> GetAll();
        Task<Brand> GetById(Guid id);
        Task<Guid> Update(Guid oldId, Brand newBrand);
    }
}