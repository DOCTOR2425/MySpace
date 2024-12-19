namespace InstrumentStore.Domain.Contracts.Cart
{
    public record AddToCartRequest(
        Guid ProductId,
        int Quantity
    );
}
