namespace InstrumentStore.Domain.DataBase.Models
{
	public class CartItem
	{
		public Guid CartItemId { get; set; }
		public int Quantity { get; set; }

		public required User User { get; set; }
		public required Product Product { get; set; }
	}
}
