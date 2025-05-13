using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.DataBase;
using InstrumentStore.Domain.DataBase.Models;
using Microsoft.EntityFrameworkCore;

namespace InstrumentStore.Domain.Services
{
	public class BrandService : IBrandService
	{
		private readonly InstrumentStoreDBContext _dbContext;

		public BrandService(InstrumentStoreDBContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<List<Brand>> GetAll()
		{
			return await _dbContext.Brand.AsNoTracking().ToListAsync();
		}

		public async Task<Brand> GetById(Guid id)
		{
			return await _dbContext.Brand.FindAsync(id);
		}

		public async Task<Guid> Create(string brandName)
		{
			Brand brand = new Brand()
			{
				BrandId = Guid.NewGuid(),
				Name = brandName
			};

			await _dbContext.Brand.AddAsync(brand);
			await _dbContext.SaveChangesAsync();

			return brand.BrandId;
		}

		public async Task<Guid> Update(Guid oldId, string newName)
		{
			await _dbContext.Brand
				.Where(p => p.BrandId == oldId)
				.ExecuteUpdateAsync(x => x
					.SetProperty(p => p.Name, newName));

			return oldId;
		}

		public async Task<Guid> Delete(Guid id)
		{
			await _dbContext.Brand
				.Where(p => p.BrandId == id)
				.ExecuteDeleteAsync();

			return id;
		}
	}
}
