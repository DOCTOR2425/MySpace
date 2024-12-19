namespace InstrumentStore.Domain.Contracts.Cart
{
    public record OrderCartRequest(
        Guid DeliveryMethodId,
        Guid PaymentMethodId
    );
}
