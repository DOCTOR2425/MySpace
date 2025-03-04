using InstrumentStore.Domain.Contracts.User;

namespace InstrumentStore.Domain.Contracts.Cart
{
	public class OrderCartRequest
	{
		public Guid DeliveryMethodId;
		public string PaymentMethod { get; set; } = string.Empty;
		public UserDelivaryAdress? UserDelivaryAdress { get; set; }
	}
}
