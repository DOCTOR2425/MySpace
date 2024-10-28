using InstrumentStore.DataAccess.Entities;
using InstrumentStore.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace InstrumentStore.DataAccess.Repository
{
	public class InstrumentRepository
	{
		private static readonly InstrumentStoreDBContext _dbContext;

		static InstrumentRepository()
		{
			_dbContext = new InstrumentStoreDBContext();
		}

		public async Task<List<Instrument>> Get()
		{
			List<InstrumentEntity> instrumentEntities = await _dbContext.Instrument
				.AsNoTracking().ToListAsync();

			List<Instrument> instruments = instrumentEntities.Select(x =>
				new Instrument(
					x.InstrumentID,
					x.Name,
					x.Description,
					x.Price,
					x.Quantity,
					x.Image,
					x.InstrumentType,
					x.Country,
					x.Supplier))
				.ToList();

			return instruments;
		}

		public async Task<Instrument> Get(int id)
		{
			InstrumentEntity? instrument = 
				_dbContext.Instrument.FirstOrDefault(x => x.InstrumentID == id);

			if (instrument == null)
				throw new ArgumentException("Не существеут инструмента с id " + id);

			return new Instrument(
				instrument.InstrumentID,
				instrument.Name,
				instrument.Description,
				instrument.Price,
				instrument.Quantity,
				instrument.Image,
				instrument.InstrumentType,
				instrument.Country,
				instrument.Supplier);
		}

		public async Task<int> Create(Instrument instrument)
		{
			var instrumentEntity = new InstrumentEntity(
				instrument.Name,
				instrument.Description,
				instrument.Price,
				instrument.Quantity,
				instrument.Image,
				instrument.Type,
				instrument.Country,
				instrument.Supplier);

			await _dbContext.Instrument.AddAsync(instrumentEntity);
			await _dbContext.SaveChangesAsync();

			return instrumentEntity.InstrumentID;
		}

		public async Task<int> Update(int instrumentID, string name,
			string description, decimal price, int quantity,
			byte[] image, int instrumentType, int country, int supplier)
		{
			InstrumentEntity? instrument = _dbContext.Instrument.FirstOrDefaultAsync(x => 
				x.InstrumentID == instrumentID).Result;

			if (instrument == null)
				throw new ArgumentNullException("Не существеут инструмента с id " + instrumentID);

			instrument.Name = name;
			instrument.Description = description;
			instrument.Price = price;
			instrument.Quantity = quantity;
			instrument.Image = image;
			instrument.InstrumentType = instrumentType;
			instrument.Country = country;
			instrument.Supplier = supplier;

			await _dbContext.SaveChangesAsync();
			return instrumentID;
		}

		public async Task<int> Delete(int id)
		{
			InstrumentEntity? instrument = _dbContext.Instrument.FirstOrDefault(x => x.InstrumentID == id);
			if (instrument == null)
				throw new ArgumentNullException("Не существеут инструмента с id " + id);

			_dbContext.Instrument.Remove(instrument);

			await _dbContext.SaveChangesAsync();
			return id;
		}
	}
}
