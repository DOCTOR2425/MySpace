using System.ComponentModel.DataAnnotations;

namespace ConsoleApp1
{
	public class DeliveryMethodEntity
	{
		[Key]
		public int DeliveryMethodID { get; set; }
		public string Name { get; set; }
		public decimal Price { get; set; }

		//public DeliveryMethodEntity(string name, decimal price)
		//{
		//	Name = name;
		//	Price = price;
		//}
	}
}
