using Microsoft.EntityFrameworkCore;

namespace InstrumentStore.DataAccess.Entities
{
	[Keyless]
	public class OrderItemEntity
	{
		public int OrderID { get; set; }
		public int InstrumentID { get; set; }
		public int Quantity { get; set; }

		public OrderItemEntity(int orderID, int instrumentID, int quantity)
		{
			OrderID = orderID;
			InstrumentID = instrumentID;
			Quantity = quantity;
		}
	}
}
