using InstrumentStore.Domain.Abstractions;

namespace InstrumentStore.Domain.Services
{
	public class PaymentMethodService : IPaymentMethodService
	{
		public static readonly Dictionary<PaymentMethod, string> PaymentMethods =
			new Dictionary<PaymentMethod, string>()
			{
				{PaymentMethod.Card, "Картой" },
				{PaymentMethod.Cash, "Наличными" }
			};

		public async Task<Dictionary<PaymentMethod, string>> GetAll()
		{
			return PaymentMethods;
		}

		public async Task<List<string>> GetAllToList()
		{
			return PaymentMethods.Values.ToList();
		}
	}
	public enum PaymentMethod
	{
		Card,
		Cash
	}
}
