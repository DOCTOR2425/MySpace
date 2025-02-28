using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.DataBase;
using InstrumentStore.Domain.DataBase.Models;
using Microsoft.EntityFrameworkCore;

namespace InstrumentStore.Domain.Services
{
	public class CityService : ICityService
	{
		private readonly InstrumentStoreDBContext _dbContext;

		public CityService(InstrumentStoreDBContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<List<City>> GetAll()
		{
			return await _dbContext.City.AsNoTracking().ToListAsync();
		}

		public async Task<City> GetById(Guid cityId)
		{
			return await _dbContext.City.FindAsync(cityId);
		}

		public async Task<Guid> Create(City city)
		{
			await _dbContext.City.AddAsync(city);
			await _dbContext.SaveChangesAsync();

			return city.CityId;
		}

		public async Task<City> GetByName(string cityName)
		{
			return await _dbContext.City.FirstOrDefaultAsync(
				c => c.Name == cityName);
		}
	}
}
