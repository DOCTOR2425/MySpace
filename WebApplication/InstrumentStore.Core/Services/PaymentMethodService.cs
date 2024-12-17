using InstrumentStore.Domain.DataBase.Models;
using InstrumentStore.Domain.DataBase;
using Microsoft.EntityFrameworkCore;
using InstrumentStore.Domain.Abstractions;

namespace InstrumentStore.Domain.Services
{
    public class PaymentMethodService : IPaymentMethodService
    {
        private readonly InstrumentStoreDBContext _dbContext;

        public PaymentMethodService(InstrumentStoreDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<PaymentMethod>> GetAll()
        {
            return await _dbContext.PaymentMethod.AsNoTracking().ToListAsync();
        }

        public async Task<PaymentMethod> GetById(Guid id)
        {
            return await _dbContext.PaymentMethod.FindAsync(id);
        }

        public async Task<Guid> Create(PaymentMethod paymentMethod)
        {
            await _dbContext.PaymentMethod.AddAsync(paymentMethod);
            await _dbContext.SaveChangesAsync();

            return paymentMethod.PaymentMethodId;
        }
    }
}
