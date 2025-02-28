using InstrumentStore.Domain.Contracts.Cart;
using InstrumentStore.Domain.DataBase.Models;
using InstrumentStore.Domain.Services;

namespace InstrumentStore.Domain.Abstractions
{
	public interface ICartService
	{
		Task<Guid> AddToCart(Guid userId, Guid productId, int quantity);
		Task<List<CartItem>> GetAllCart(Guid userId);
		Task<Guid> OrderCartForLogined(Guid userId, Guid deliveryMethodId, string paymentMethod);
		Task<Guid> OrderProduct(Guid userId,
			Guid productId,
			int quantity,
			Guid deliveryMethodId,
			string paymentMethod);
		Task<Guid> RemoveFromCart(Guid cartItemId);
		Task<List<PaidOrderItem>> GetAllOrders(Guid userId);
		Task<Guid> OrderCartForUnlogined(Guid userId, OrderCartOfUnregisteredRequest request);
	}
}
