using InstrumentStore.Domain.DataBase.Models;

namespace InstrumentStore.Domain.Abstractions
{
	public interface IFillDataBaseService
	{
		Task ClearDatabase();
		Task<List<Product>> CreateBosch();
		Task<List<Product>> CreateDewalt();
		Task<List<Product>> CreateMakita();
		Task FillAll();
		Task FillProducts();
	}
}