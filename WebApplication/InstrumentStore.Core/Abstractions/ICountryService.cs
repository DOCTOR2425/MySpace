using InstrumentStore.Domain.DataBase.Models;

namespace InstrumentStore.Domain.Abstractions
{
    public interface ICountryService
    {
        Task<Guid> Create(Country brand);
        Task<Guid> Delete(Guid id);
        Task<List<Country>> GetAll();
        Task<Country> GetById(Guid id);
        Task<Guid> Update(Guid oldId, Country newCountry);
    }
}