using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.DataBase;
using InstrumentStore.Domain.DataBase.Models;
using Microsoft.EntityFrameworkCore;

namespace InstrumentStore.Domain.Services
{
    public class CountryService : ICountryService
    {
        private readonly InstrumentStoreDBContext _dbContext;

        public CountryService(InstrumentStoreDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Country>> GetAll()
        {
            return await _dbContext.Country.AsNoTracking().ToListAsync();
        }

        public async Task<Country> GetById(Guid id)
        {
            return await _dbContext.Country.FindAsync(id);
        }

        public async Task<Guid> Create(Country brand)
        {
            await _dbContext.Country.AddAsync(brand);
            await _dbContext.SaveChangesAsync();

            return brand.CountryId;
        }

        public async Task<Guid> Update(Guid oldId, Country newCountry)
        {
            await _dbContext.Country
                .Where(p => p.CountryId == oldId)
                .ExecuteUpdateAsync(x => x
                    .SetProperty(p => p.Name, newCountry.Name));

            _dbContext.SaveChanges();

            return oldId;
        }

        public async Task<Guid> Delete(Guid id)
        {
            await _dbContext.Country
                .Where(p => p.CountryId == id)
                .ExecuteDeleteAsync();

            return id;
        }
    }
}
