using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.DataBase;
using InstrumentStore.Domain.DataBase.Models;
using Microsoft.EntityFrameworkCore;

namespace InstrumentStore.Domain.Service
{
    public class ProductService : IProductService
    {
        private readonly InstrumentStoreDBContext _dbContext;

        public ProductService(InstrumentStoreDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Product>> GetAll()
        {
            return await _dbContext.Product
                .Include(p => p.ProductType)
                .Include(p => p.Brand)
                .Include(p => p.Country).AsNoTracking().ToListAsync();
        }

        public async Task<Product> GetById(Guid id)
        {
            return await _dbContext.Product
                .Include(p => p.ProductType)
                .Include(p => p.Brand)
                .Include(p => p.Country)
                .Where(p => p.ProductId == id)
                .FirstOrDefaultAsync();
        }

        public async Task<Guid> Create(Product product)
        {
            await _dbContext.Product.AddAsync(product);
            await _dbContext.SaveChangesAsync();

            return product.ProductId;
        }

        public async Task<Guid> Update(Guid oldId, Product newProduct)
        {
            await _dbContext.Product
                .Where(p => p.ProductId == oldId)
                .ExecuteUpdateAsync(x => x
                    .SetProperty(p => p.ProductType, newProduct.ProductType)
                    .SetProperty(p => p.Brand, newProduct.Brand)
                    .SetProperty(p => p.Country, newProduct.Country)
                    .SetProperty(p => p.Description, newProduct.Description)
                    .SetProperty(p => p.Name, newProduct.Name)
                    .SetProperty(p => p.Image, newProduct.Image)
                    .SetProperty(p => p.Price, newProduct.Price)
                    .SetProperty(p => p.Quantity, newProduct.Quantity));

            _dbContext.SaveChanges();

            return oldId;
        }

        public async Task<Guid> Delete(Guid id)
        {
            await _dbContext.Product
                .Where(p => p.ProductId == id)
                .ExecuteDeleteAsync();

            return id;
        }
    }
}
