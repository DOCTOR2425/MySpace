using InstrumentStore.Domain.Contracts.User;

namespace InstrumentStore.Domain.Contracts.PaidOrders
{
	public class AdminPaidOrderResponse
	{
		public Guid PaidOrderId { get; set; }
		public DateTime OrderDate { get; set; }
		public UserOrderInfo UserOrderInfo { get; set; }
		public List<PaidOrderItemResponse> PaidOrderItems { get; set; }
	}
}
