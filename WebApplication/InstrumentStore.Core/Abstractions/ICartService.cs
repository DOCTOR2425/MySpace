using InstrumentStore.Domain.Contracts.Cart;
using InstrumentStore.Domain.DataBase.Models;

namespace InstrumentStore.Domain.Abstractions
{
	public interface ICartService
	{
		Task<Guid> AddToCart(Guid userId, Guid productId, int quantity);
		Task<List<CartItem>> GetAllCart(Guid userId);
		Task<Guid> OrderCartForRegistered(Guid userId, OrderRequest orderCartRequest);
		Task<Guid> OrderProduct(Guid userId,
			Guid productId,
			int quantity,
			OrderRequest orderRequest);
		Task<Guid> RemoveFromCart(Guid cartItemId);
		Task<List<PaidOrderItem>> GetAllOrders(Guid userId);
		Task<Guid> OrderCartForUnregistered(Guid userId, OrderCartOfUnregisteredRequest request);
	}
}
