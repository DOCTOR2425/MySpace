using InstrumentStore.DataAccess.Repository;
using InstrumentStore.Domain.Models;

namespace InstrumentStore.Application.Services
{
	public class InstrumentServices
	{
		private static readonly InstrumentRepository _instrumentRepository;

		static InstrumentServices()
		{
			_instrumentRepository = new InstrumentRepository();
		}

		public async Task<List<Instrument>> GetAllInstruments()
		{
			return await _instrumentRepository.Get();
		}

		public async Task<Instrument> GetInstrument(int id)
		{
			return await _instrumentRepository.Get(id);
		}

		public async Task<int> CreateInstrument(Instrument instrument)
		{
			if (instrument == null || IsFieldsFalid(instrument) == false)
				throw new ArgumentException("Данные об инструменте не заполненны или заполненны неправильно");

			return await _instrumentRepository.Create(instrument);
		}

		public async Task<int> DeleteInstrument(int id)
		{
			return await _instrumentRepository.Delete(id);
		}

		public async Task<int> UpdateInstrument(int instrumentID, string name,
			string description, decimal price, int quantity,
			byte[] image, int instrumentType, int country, int supplier)
		{
			if (IsFieldsFalid(new Instrument(instrumentID, name, description, 
				price, quantity, image, instrumentType, country, supplier)) == false)
				throw new ArgumentException("Данные для изменения заполненны неверно");

			return await _instrumentRepository.Update(instrumentID, name, description,
				price, quantity, image, instrumentType, country, supplier);
		}

		private bool IsFieldsFalid(Instrument instrument)
		{
			if (instrument == null)
				return false;

			if (instrument.Name == null ||
				instrument.Name.Length == 0)
				return false;

			if (instrument.Description == null ||
				instrument.Description.Length == 0)
				return false;

			if (instrument.Image == null ||
				instrument.Image.Length == 0)
				return false;

			if (instrument.Price <= 0)
				return false;

			return true;
		}
	}
}
