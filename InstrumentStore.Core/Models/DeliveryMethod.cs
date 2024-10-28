namespace InstrumentStore.Domain.Models
{
	public class DeliveryMethod
	{
		public int DeliveryMethodID { get; set; }
		public string Name { get; set; }
		public decimal Price { get; set; }

		public DeliveryMethod(int deliveryMethodID, string name, decimal price)
		{
			DeliveryMethodID = deliveryMethodID;
			Name = name;
			Price = price;
		}
	}
}
