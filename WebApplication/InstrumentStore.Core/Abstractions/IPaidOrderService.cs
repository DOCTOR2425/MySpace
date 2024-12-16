using InstrumentStore.Domain.DataBase.Models;

namespace InstrumentStore.Domain.Abstractions
{
    public interface IPaidOrderService
    {
        Task<Guid> Create(Guid userId, Guid deliveryMethodId, Guid paymentMethodId);
        Task<List<PaidOrder>> GetAll(Guid userId);
        Task<PaidOrder> GetById(Guid orderId);
    }
}