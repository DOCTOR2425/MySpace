using InstrumentStore.Domain.Contracts.User;
using InstrumentStore.Domain.DataBase.Models;

namespace InstrumentStore.Domain.Contracts.PaidOrders
{
	public class AdminPaidOrderResponse
	{
		public Guid PaidOrderId { get; set; }
		public DateTime OrderDate { get; set; }
		public DateTime ReceiptDate { get; set; } = DateTime.MinValue;
		public string PaymentMethod { get; set; } = string.Empty;
		public DeliveryMethod DeliveryMethod { get; set; }
		public UserOrderInfo UserOrderInfo { get; set; }
		public List<PaidOrderItemResponse> PaidOrderItems { get; set; }
		public PromoCode? PromoCode { get; set; }
	}
}
