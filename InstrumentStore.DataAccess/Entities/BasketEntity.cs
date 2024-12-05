using System.ComponentModel.DataAnnotations;

namespace InstrumentStore.DataAccess.Entities
{
	public class BasketEntity
	{
		[Key]
		public int BasketID { get; set; }
		public int Cost { get; set; }
		public int Customer { get; set; }

		public BasketEntity(int basketID, int cost, int customer)
		{
			BasketID = basketID;
			Cost = cost;
			Customer = customer;
		}

		public BasketEntity(int cost, int customer)
		{
			Cost = cost;
			Customer = customer;
		}
	}
}
