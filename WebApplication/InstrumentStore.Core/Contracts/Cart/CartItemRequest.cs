using InstrumentStore.Domain.Contracts.Products;

namespace InstrumentStore.Domain.Contracts.Cart
{
    public record CartItemRequest(
         Guid CartItemId,
         ProductData Product,
         int Quantity
    );
}
