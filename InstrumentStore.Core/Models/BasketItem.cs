namespace InstrumentStore.Domain.Models
{
	public class BasketItem
	{
		public int BasketID { get; set; }
		public int InstrumentID { get; set; }
		public int Quantity { get; set; }

		public BasketItem(int basketID, int instrumentID, int quantity)
		{
			BasketID = BasketID;
			InstrumentID = instrumentID;
			Quantity = quantity;
		}
	}
}
