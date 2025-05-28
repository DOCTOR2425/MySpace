using InstrumentStore.Domain.Contracts.User;

namespace InstrumentStore.Domain.Contracts.Cart
{
	public class OrderRequest
	{
		public Guid DeliveryMethodId { get; set; }
		public string PaymentMethod { get; set; } = string.Empty;
		public UserDeliveryAddress? UserDelivaryAddress { get; set; }
		public string PromoCode { get; set; } = string.Empty;
	}
}
