using InstrumentStore.Domain.Contracts.Cart;
using InstrumentStore.Domain.Contracts.Products;
using InstrumentStore.Domain.DataBase.Models;

namespace InstrumentStore.Domain.Abstractions
{
	public interface ICartService
	{
		Task<Guid> AddToCart(Guid userId, Guid productId, int quantity);
		Task<List<CartItem>> GetUserCartItems(Guid userId);
		Task<Guid> OrderCartForRegistered(Guid userId, OrderRequest orderCartRequest);
		Task<Guid> OrderProduct(Guid userId,
			Guid productId,
			int quantity,
			OrderRequest orderRequest);
		Task<Guid> RemoveFromCart(Guid cartItemId, Guid userId);
		Task<List<PaidOrderItem>> GetUserOrders(Guid userId);
		Task<Guid> OrderCartForUnregistered(Guid userId, OrderCartOfUnregisteredRequest request);
		Task<int> GetProductQuantityInUserCart(Guid productId, Guid userId);
		Task<CartItemResponse> GetCartItemResponse(CartItem cartItem);
		Task<List<ProductMinimalData>> GetProductForUnregestereCart(List<Guid> productsId);
	}
}
