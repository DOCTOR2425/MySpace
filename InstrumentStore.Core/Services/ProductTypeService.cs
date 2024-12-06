using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.DataBase;
using InstrumentStore.Domain.DataBase.Models;
using Microsoft.EntityFrameworkCore;

namespace InstrumentStore.Domain.Services
{
    public class ProductTypeService : IProductTypeService
    {
        private readonly InstrumentStoreDBContext _dbContext;

        public ProductTypeService(InstrumentStoreDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<ProductType>> GetAll()
        {
            return await _dbContext.ProductType.AsNoTracking().ToListAsync();
        }

        public async Task<ProductType> GetById(Guid id)
        {
            return await _dbContext.ProductType.FindAsync(id);
        }

        public async Task<Guid> Create(ProductType brand)
        {
            await _dbContext.ProductType.AddAsync(brand);
            await _dbContext.SaveChangesAsync();

            return brand.ProductTypeId;
        }

        public async Task<Guid> Update(Guid oldId, ProductType newProductType)
        {
            await _dbContext.ProductType
                .Where(p => p.ProductTypeId == oldId)
                .ExecuteUpdateAsync(x => x
                    .SetProperty(p => p.Name, newProductType.Name));

            _dbContext.SaveChanges();

            return oldId;
        }

        public async Task<Guid> Delete(Guid id)
        {
            await _dbContext.ProductType
                .Where(p => p.ProductTypeId == id)
                .ExecuteDeleteAsync();

            return id;
        }
    }
}
