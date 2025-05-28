using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.DataBase;
using InstrumentStore.Domain.DataBase.Models;
using Microsoft.EntityFrameworkCore;

namespace InstrumentStore.Domain.Services
{
	public class PromoCodeService : IPromoCodeService
	{
		private readonly InstrumentStoreDBContext _dbContext;

		public PromoCodeService(InstrumentStoreDBContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<List<PromoCode>> GetAll()
		{
			return await _dbContext.PromoCode.ToListAsync();
		}

		public async Task<PromoCode?> Get(Guid promoId)
		{
			return await _dbContext.PromoCode.FindAsync(promoId);
		}

		public async Task<PromoCode?> Get(string promoName)
		{
			return await _dbContext.PromoCode.FirstOrDefaultAsync(
				p => p.Name == promoName);
		}

		public async Task<Guid> Create(string name, decimal amount)
		{
			PromoCode promoCode = new PromoCode()
			{
				PromoCodeId = Guid.NewGuid(),
				Name = name,
				Amount = amount
			};
			await _dbContext.PromoCode.AddAsync(promoCode);

			await _dbContext.SaveChangesAsync();
			return promoCode.PromoCodeId;
		}

		public async Task<PromoCode?> GetIfActive(Guid promoId)
		{
			PromoCode? promoCode = await Get(promoId);
			if (promoCode.IsActive)
				return promoCode;
			return null;
		}

		public async Task<PromoCode?> GetIfActive(string promoName)
		{
			PromoCode? promoCode = await Get(promoName);
			if (promoCode != null && promoCode.IsActive)
				return promoCode;
			return null;
		}

		public async Task<Guid> ChangeStatus(Guid promoId, bool status)
		{
			await _dbContext.PromoCode
				.Where(p => p.PromoCodeId == promoId)
				.ExecuteUpdateAsync(p => p
					.SetProperty(p => p.IsActive, status));

			return promoId;
		}

		public async Task<Guid> ChangeStatus(string promoName, bool status)
		{
			PromoCode? promoCode = await Get(promoName);
			if (promoCode == null)
				throw new ArgumentException("No promo code with that name");

			promoCode.IsActive = status;

			return promoCode.PromoCodeId;
		}
	}
}
