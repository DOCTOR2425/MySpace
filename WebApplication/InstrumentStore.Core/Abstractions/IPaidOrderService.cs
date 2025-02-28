using InstrumentStore.Domain.DataBase.Models;
using InstrumentStore.Domain.Services;

namespace InstrumentStore.Domain.Abstractions
{
	public interface IPaidOrderService
	{
		Task<Guid> Create(Guid userId, Guid deliveryMethodId, string paymentMethod);
		Task<List<PaidOrder>> GetAll(Guid userId);
		Task<PaidOrder> GetById(Guid orderId);
		Task<List<PaidOrderItem>> GetAllItemsByOrder(Guid paidOrderId);
	}
}