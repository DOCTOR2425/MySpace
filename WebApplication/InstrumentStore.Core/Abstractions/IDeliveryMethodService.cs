using InstrumentStore.Domain.DataBase.Models;

namespace InstrumentStore.Domain.Abstractions
{
    public interface IDeliveryMethodService
    {
        Task<List<DeliveryMethod>> GetAll();
        Task<DeliveryMethod> GetById(Guid deliveryMethodId);
        Task<Guid> Create(DeliveryMethod deliveryMethod);
        Task<bool> IsHomeDelivery(Guid deliveryMethodId);
    }
}