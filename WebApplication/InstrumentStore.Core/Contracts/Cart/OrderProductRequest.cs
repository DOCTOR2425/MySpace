namespace InstrumentStore.Domain.Contracts.Cart
{
	public record OrderProductRequest(
		Guid ProductId,
		int Quantity,
		OrderRequest OrderCartRequest
	);
}
