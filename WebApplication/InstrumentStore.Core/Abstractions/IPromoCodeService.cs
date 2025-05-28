using InstrumentStore.Domain.DataBase.Models;

namespace InstrumentStore.Domain.Abstractions
{
	public interface IPromoCodeService
	{
		Task<Guid> ChangeStatus(Guid promoId, bool status);
		Task<Guid> ChangeStatus(string promoName, bool status);
		Task<Guid> Create(string name, decimal amount);
		Task<PromoCode?> Get(Guid promoId);
		Task<PromoCode?> Get(string promoName);
		Task<PromoCode?> GetIfActive(Guid promoId);
		Task<PromoCode?> GetIfActive(string promoName);
		Task<List<PromoCode>> GetAll();
	}
}