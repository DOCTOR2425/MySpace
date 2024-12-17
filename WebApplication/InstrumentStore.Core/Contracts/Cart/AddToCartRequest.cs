namespace InstrumentStore.Domain.Contracts.Cart
{
    public record AddToCartRequest(
        Guid UserId,
        Guid ProductId,
        int Quantity
    );
}
