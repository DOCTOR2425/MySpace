namespace InstrumentStore.Domain.Models
{
	public class PaymentMethod
	{
		public int PaymentMethodID { get; set; }
		public string Name { get; set;}

		public PaymentMethod(int paymentMethodID, string name)
		{
			PaymentMethodID = paymentMethodID;
			Name = name;
		}
	}
}
