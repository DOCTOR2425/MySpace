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

        public async Task<DeliveryMethod> GetById(Guid id)
        {
            return await _dbContext.DeliveryMethod.FindAsync(id);
        }
    }
}
