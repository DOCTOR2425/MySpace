using InstrumentStore.Domain.Contracts.Cart;
using InstrumentStore.Domain.DataBase.Models;

namespace InstrumentStore.Domain.Abstractions
{
    public interface ICartService
    {
        Task<Guid> AddToCart(Guid userId, Guid productId, int quantity);
        Task<List<CartItem>> GetAllCart(Guid userId);
        Task<Guid> OrderCart(Guid userId, Guid deliveryMethodId, Guid paymentMethodId);
        Task<Guid> OrderProduct(Guid userId, Guid productId, int quantity, Guid deliveryMethodId, Guid paymentMethodId);
        Task<Guid> RemoveFromCart(Guid cartItemId);
        Task<List<PaidOrderItem>> GetAllOrders(Guid userId);
    }
}
