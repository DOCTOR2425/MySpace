namespace InstrumentStore.Domain.Contracts.PaidOrders
{
	public class PaidOrderResponse
	{
		public Guid PaidOrderId { get; set; }
		public DateTime OrderDate { get; set; }
		public List<PaidOrderItemResponse> PaidOrderItems { get; set; }
	}
}
