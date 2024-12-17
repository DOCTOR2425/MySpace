namespace InstrumentStore.Domain.Contracts.Cart
{
    public record OrderProductRequest(
        Guid UserId,
        Guid ProductId,
        Guid DeliveryMethodId,
        Guid PaymentMethodId
    );
}
