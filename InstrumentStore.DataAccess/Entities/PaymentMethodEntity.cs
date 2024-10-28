using System.ComponentModel.DataAnnotations;

namespace InstrumentStore.DataAccess.Entities
{
	public class PaymentMethodEntity
	{
		[Key]
		public int PaymentMethodID { get; set; }
		public string Name { get; set;}

		public PaymentMethodEntity(string name)
		{
			Name = name;
		}

		public PaymentMethodEntity(int paymentMethodID, string name)
		{
			PaymentMethodID = paymentMethodID;
			Name = name;
		}
	}
}
