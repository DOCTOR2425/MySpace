namespace InstrumentStore.Domain.Models
{
	public class OrderItem
	{
		public int OrderID { get; set; }
		public int InstrumentID { get; set; }
		public int Quantity { get; set; }

		public OrderItem(int orderID, int instrumentID, int quantity)
		{
			OrderID = orderID;
			InstrumentID = instrumentID;
			Quantity = quantity;
		}
	}
}
