using InstrumentStore.Domain.DataBase.Models;

namespace InstrumentStore.Domain.Abstractions
{
	public interface IBrandService
	{
		Task<Guid> Create(string brandName);
		Task<Guid> Delete(Guid id);
		Task<List<Brand>> GetAll();
		Task<Brand> GetById(Guid id);
		Task<Guid> Update(Guid oldId, string newName);
	}
}