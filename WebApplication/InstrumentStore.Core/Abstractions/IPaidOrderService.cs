using InstrumentStore.Domain.DataBase.Models;

namespace InstrumentStore.Domain.Abstractions
{
	public interface IPaidOrderService
	{
		Task<Guid> Create(Guid userId, Guid deliveryMethodId, string paymentMethod);
		Task<List<PaidOrder>> GetAll(Guid userId);
		Task<PaidOrder> GetById(Guid orderId);
		Task<List<PaidOrderItem>> GetAllItemsByOrder(Guid paidOrderId);
		Task<Guid> CloseOrder(Guid orderId);
		Task<List<PaidOrder>> GetProcessingOrders();
		Task<Guid> CancelOrder(Guid orderId);
	}
}