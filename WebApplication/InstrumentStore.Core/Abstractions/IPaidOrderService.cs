using InstrumentStore.Domain.Contracts.Cart;
using InstrumentStore.Domain.DataBase.Models;

namespace InstrumentStore.Domain.Abstractions
{
    public interface IPaidOrderService
    {
        Task<Guid> Create(Guid userId, OrderRequest orderCartRequest);
        Task<List<PaidOrder>> GetAllByUserId(Guid userId);
        Task<PaidOrder> GetById(Guid orderId);
        Task<List<PaidOrderItem>> GetAllItemsByOrder(Guid paidOrderId);
        Task<Guid> CloseOrder(Guid orderId);
        Task<List<PaidOrder>> GetProcessingOrders();
        Task<Guid> CancelOrder(Guid orderId);
        Task<DeliveryAddress?> GetDeliveryAddressByOrderId(Guid orderId);
        Task<List<PaidOrder>> GetAll(int page);

    }
}