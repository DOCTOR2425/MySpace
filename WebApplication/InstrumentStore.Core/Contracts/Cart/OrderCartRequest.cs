namespace InstrumentStore.Domain.Contracts.Cart
{
    public record OrderCartRequest(
        Guid UserId,
        Guid DeliveryMethodId,
        Guid PaymentMethodId
    );
}
