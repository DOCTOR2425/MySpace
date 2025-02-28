using InstrumentStore.Domain.Services;

namespace InstrumentStore.Domain.Abstractions
{
	public interface IPaymentMethodService
	{
		Task<Dictionary<PaymentMethod, string>> GetAll();
		Task<List<string>> GetAllToList();
	}
}