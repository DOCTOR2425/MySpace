using InstrumentStore.Domain.Contracts.Products;

namespace InstrumentStore.Domain.Contracts.Cart
{
	public record CartItemResponse(
		 Guid CartItemId,
		 ProductResponse Product,
		 int Quantity
	);
}
