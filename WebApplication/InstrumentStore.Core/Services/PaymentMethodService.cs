using InstrumentStore.Domain.Abstractions;

namespace InstrumentStore.Domain.Services
{
	public class PaymentMethodService : IPaymentMethodService
	{
		public static readonly Dictionary<PaymentMethod, string> PaymentMethods =
			new Dictionary<PaymentMethod, string>()
			{
				{PaymentMethod.ERIP, "ERIP перевод" },
				{PaymentMethod.Cash, "Наличными при получении" }
			};

		public async Task<Dictionary<PaymentMethod, string>> GetAll()
		{
			return new Dictionary<PaymentMethod, string>()
			{
				{PaymentMethod.ERIP, "ERIP перевод" },
				{PaymentMethod.Cash, "Наличными при получении" }
			};
		}

		public async Task<List<string>> GetAllToList()
		{
			return new List<string>()
			{
				"ERIP перевод",
				"Наличными при получении"
			};
		}
	}
	public enum PaymentMethod
	{
		ERIP,
		Cash
	}
}
