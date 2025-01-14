using InstrumentStore.Domain.Contracts.User;

namespace InstrumentStore.Domain.Contracts.Cart
{
    public record OrderCartOfUnregisteredRequest(
        RegisterUserFromOrderRequest User,
        AddToCartRequest[] CartItems,
        Guid DeliveryMethodId,
        Guid PaymentMethodId);
}
