namespace InstrumentStore.Domain.Contracts.Cart
{
	public class CartItemResponse
	{
		public Guid ProductId { get; set; }
		public string ProductName { get; set; } = string.Empty;
		public decimal ProductPrice { get; set; }
		public string ProductImage { get; set; } = string.Empty;
		public bool IsProductArchive { get; set; } = false;

		public int Quantity { get; set; }
	}
}
