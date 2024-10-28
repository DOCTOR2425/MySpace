namespace InstrumentStore.Domain.Models
{
	public class tbl_Order
	{
		public int OrderID { get; set; }
		public string CustomerCity { get; set; }
		public string CustomerAddress { get; set; }
		public DateTime RegistrationDate { get; set; }
		public DateTime DeliveryDate { get; set; }
		public int DeliveryMethod { get; set; }
		public int PaymentMethod { get; set; }
		public int Customer { get; set; }

		public tbl_Order(int orderID, string customerCity, 
			string customerAddress, DateTime registrationDate, 
			DateTime deliveryDate, int deliveryMethod, 
			int paymentMethod, int customer)
		{
			OrderID = orderID;
			CustomerCity = customerCity;
			CustomerAddress = customerAddress;
			RegistrationDate = registrationDate;
			DeliveryDate = deliveryDate;
			DeliveryMethod = deliveryMethod;
			PaymentMethod = paymentMethod;
			Customer = customer;
		}
	}
}
