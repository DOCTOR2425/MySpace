using Microsoft.EntityFrameworkCore;

namespace ConsoleApp1
{
	[Keyless]
	public class OrderItemEntity
	{
		public int OrderId { get; set; }
		public int InstrumentId { get; set; }
		public int Quantity { get; set; }

		//public OrderItemEntity(int orderID, int instrumentID, int quantity)
		//{
		//	OrderId = orderID;
		//	InstrumentId = instrumentID;
		//	Quantity = quantity;
		//}
	}
}
