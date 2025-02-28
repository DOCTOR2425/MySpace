using InstrumentStore.Domain.DataBase.Models;

namespace InstrumentStore.Domain.Abstractions
{
	public interface ICityService
	{
		Task<Guid> Create(City city);
		Task<List<City>> GetAll();
		Task<City> GetById(Guid cityId);
		Task<City> GetByName(string cityName);
	}
}