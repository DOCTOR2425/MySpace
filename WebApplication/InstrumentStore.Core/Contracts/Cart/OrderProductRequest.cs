namespace InstrumentStore.Domain.Contracts.Cart
{
    public record OrderProductRequest(
        Guid ProductId,
        Guid DeliveryMethodId,
        Guid PaymentMethodId
    );
}
