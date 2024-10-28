using System.ComponentModel.DataAnnotations;

namespace InstrumentStore.DataAccess.Entities
{
	public class DeliveryMethodEntity
	{
		[Key]
		public int DeliveryMethodID { get; set; }
		public string Name { get; set; }
		public decimal Price { get; set; }

		public DeliveryMethodEntity(string name, decimal price)
		{
			Name = name;
			Price = price;
		}

		public DeliveryMethodEntity(int deliveryMethodID, string name, decimal price)
		{
			DeliveryMethodID = deliveryMethodID;
			Name = name;
			Price = price;
		}
	}
}
