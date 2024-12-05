using Microsoft.EntityFrameworkCore;

namespace InstrumentStore.DataAccess.Entities
{
	[Keyless]
	public class BasketItemEntity
	{
		public int BasketID { get; set; }
		public int InstrumentID { get; set; }
		public int Quantity { get; set; }

		public BasketItemEntity(int basketID, int instrumentID, int quantity)
		{
			BasketID = BasketID;
			InstrumentID = instrumentID;
			Quantity = quantity;
		}
	}
}
