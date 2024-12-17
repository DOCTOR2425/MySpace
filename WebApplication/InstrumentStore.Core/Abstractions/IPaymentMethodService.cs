using InstrumentStore.Domain.DataBase.Models;

namespace InstrumentStore.Domain.Abstractions
{
    public interface IPaymentMethodService
    {
        Task<List<PaymentMethod>> GetAll();
        Task<PaymentMethod> GetById(Guid id);
        Task<Guid> Create(PaymentMethod paymentMethod);
    }
}