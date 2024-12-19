namespace InstrumentStore.Domain.Contracts.Cart
{
    public record OrderProductRequest(
        Guid ProductId,
        int Quantity,
        Guid DeliveryMethodId,
        Guid PaymentMethodId
    );
}
