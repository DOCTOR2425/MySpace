using InstrumentStore.Domain.Contracts.Products;

namespace InstrumentStore.Domain.Contracts.PaidOrders
{
	public class PaidOrderItemResponse
	{
		public Guid PaidOrderItemId { get; set; }
		public int Quantity { get; set; }
		public decimal Price { get; set; }

		public required ProductData ProductData { get; set; }
	}
}
