using InstrumentStore.Domain.Models;

namespace InstrumentStore.Domain.Abstractions
{
	public interface IInstrumentServices
	{
		Task<int> CreateInstrument(Instrument instrument);
		Task<int> DeleteInstrument(int id);
		Task<List<Instrument>> GetAllInstruments();
		//Task<int> UpdateInstrument(int id, string name, TypesOfInstrument type, string description, decimal price, string image);
	}
}