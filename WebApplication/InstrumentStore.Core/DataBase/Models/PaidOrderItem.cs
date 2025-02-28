namespace InstrumentStore.Domain.DataBase.Models
{
	public class PaidOrderItem
	{
		public Guid PaidOrderItemId { get; set; }
		public int Quantity { get; set; }
		public decimal Price { get; set; }

		public required PaidOrder PaidOrder { get; set; }
		public required Product Product { get; set; }
	}
}
