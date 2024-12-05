using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstrumentStore.Domain.Models
{
	public class Basket
	{
		public int BasketID { get; set; }
		public int Cost { get; set; }
		public int Customer { get; set; }

		public Basket(int basketID, int cost, int customer)
		{
			BasketID = basketID;
			Cost = cost;
			Customer = customer;
		}
	}
}
