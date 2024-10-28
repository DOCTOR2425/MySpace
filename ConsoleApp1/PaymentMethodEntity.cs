using System.ComponentModel.DataAnnotations;

namespace ConsoleApp1
{
	public class PaymentMethodEntity
	{
		[Key]
		public int PaymentMethodID { get; set; }
		public string Name { get; set;}

		//public PaymentMethodEntity(string name)
		//{
		//	Name = name;
		//}
	}
}
