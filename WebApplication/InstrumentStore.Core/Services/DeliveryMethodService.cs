using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.DataBase;
using InstrumentStore.Domain.DataBase.Models;
using Microsoft.EntityFrameworkCore;

namespace InstrumentStore.Domain.Services
{
    public class DeliveryMethodService : IDeliveryMethodService
    {
        private readonly InstrumentStoreDBContext _dbContext;

        public DeliveryMethodService(InstrumentStoreDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<DeliveryMethod>> GetAll()
        {
            return await _dbContext.DeliveryMethod.AsNoTracking().ToListAsync();
        }

        public async Task<DeliveryMethod> GetById(Guid deliveryMethodId)
        {
            return await _dbContext.DeliveryMethod.FindAsync(deliveryMethodId);
        }

        public async Task<Guid> Create(DeliveryMethod deliveryMethod)
        {
            await _dbContext.DeliveryMethod.AddAsync(deliveryMethod);
            await _dbContext.SaveChangesAsync();

            return deliveryMethod.DeliveryMethodId;
        }

        public async Task<bool> IsHomeDelivery(Guid deliveryMethodId)
        {
            if((await this.GetById(deliveryMethodId)).DeliveryMethodId.ToString()
                == "5066ce29-5821-41e8-965a-56e6a18aaa8f")
                return true;
            return false;
        }
    }
}
