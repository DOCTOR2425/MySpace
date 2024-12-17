using InstrumentStore.Domain.Contracts.Cart;
using InstrumentStore.Domain.DataBase.Models;

namespace InstrumentStore.Domain.Abstractions
{
    public interface ICartService
    {
        Task<Guid> AddToCart(AddToCartRequest request);
        Task<List<CartItem>> GetAll(Guid userId);
        Task<Guid> OrderCart(Guid userId, Guid deliveryMethodId, Guid paymentMethodId);
        Task<Guid> OrderProduct(Guid userId, Guid productId, Guid deliveryMethodId, Guid paymentMethodId);
        Task<Guid> RemoveFromCart(Guid cartItemId);
    }
}
