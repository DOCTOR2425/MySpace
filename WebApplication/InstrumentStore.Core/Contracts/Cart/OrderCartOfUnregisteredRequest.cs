using InstrumentStore.Domain.Contracts.User;

namespace InstrumentStore.Domain.Contracts.Cart
{
	public class OrderCartOfUnregisteredRequest
	{
		public Guid DeliveryMethodId { get; set; }
		public string PaymentMethod { get; set; } = string.Empty;
		public required RegisterUserFromOrderRequest User { get; set; }
		public required AddToCartRequest[] CartItems { get; set; }

		public UserDeliveryAddress? UserDelivaryAddress { get; set; }
		public string PromoCode { get; set; } = string.Empty;
	}
}
