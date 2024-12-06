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
            Product product = await _dbContext.Product.FindAsync(oldId);

            product.Name = newProduct.Name;
            product.ProductType = newProduct.ProductType;
            product.Brand = newProduct.Brand;
            product.Country = newProduct.Country;
            product.Description = newProduct.Description;
            product.Image = newProduct.Image;
            product.Price = newProduct.Price;
            product.Quantity = newProduct.Quantity;

            await _dbContext.SaveChangesAsync();

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
