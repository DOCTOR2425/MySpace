using InstrumentStore.Domain.DataBase.Models;

namespace InstrumentStore.Domain.Abstractions
{
	public interface IFillDataBaseService
	{
		Task ClearDatabase();
		Task FillAll();
	}
}