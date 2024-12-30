using System.Linq;
using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.Contracts.Products;
using InstrumentStore.Domain.DataBase;
using InstrumentStore.Domain.DataBase.Models;
using Microsoft.EntityFrameworkCore;

namespace InstrumentStore.Domain.Service
{
    public class ProductService : IProductService
    {
        private readonly InstrumentStoreDBContext _dbContext;
        private readonly IBrandService _brandService;
        private readonly ICountryService _countryService;
        private readonly IProductTypeService _productTypeService;

        public ProductService(InstrumentStoreDBContext dbContext, 
            IBrandService brandService,
            ICountryService countryService, 
            IProductTypeService productTypeService)
        {
            _dbContext = dbContext;
            _brandService = brandService;
            _countryService = countryService;
            _productTypeService = productTypeService;
        }

        public async Task<List<Product>> GetAll()
        {
            return await _dbContext.Product
                .Include(p => p.ProductCategory)
                .Include(p => p.Brand)
                .Include(p => p.Country)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Product> GetById(Guid id)
        {
            return await _dbContext.Product
                .Include(p => p.ProductCategory)
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

        public async Task<Guid> Create(ProductRequest productRequest)
        {
            Product product = new Product
            {
                ProductId = Guid.NewGuid(),
                Name = productRequest.Name,
                Description = productRequest.Description,
                Price = productRequest.Price,
                Quantity = productRequest.Quantity,
                Image = productRequest.Image,

                ProductCategory = await _productTypeService.GetById(productRequest.ProductTypeId),
                Brand = await _brandService.GetById(productRequest.BrandId),
                Country = await _countryService.GetById(productRequest.CountryId)
            };

            await _dbContext.Product.AddAsync(product);
            await _dbContext.SaveChangesAsync();

            return product.ProductId;
        }

        public async Task<Guid> Update(Guid oldId, Product newProduct)
        {
            Product product = await _dbContext.Product.FindAsync(oldId);

            product.Name = newProduct.Name;
            product.ProductCategory = newProduct.ProductCategory;
            product.Brand = newProduct.Brand;
            product.Country = newProduct.Country;
            product.Description = newProduct.Description;
            product.Image = newProduct.Image;
            product.Price = newProduct.Price;
            product.Quantity = newProduct.Quantity;

            await _dbContext.SaveChangesAsync();

            return oldId;
        }

        public async Task<Guid> Update(Guid oldId, ProductRequest newProduct)
        {
            Product product = await _dbContext.Product.FindAsync(oldId);

            product.Name = newProduct.Name;
            product.Description = newProduct.Description;
            product.Image = newProduct.Image;
            product.Price = newProduct.Price;
            product.Quantity = newProduct.Quantity;
            product.ProductCategory = await _productTypeService.GetById(newProduct.ProductTypeId);
            product.Brand = await _brandService.GetById(newProduct.BrandId);
            product.Country = await _countryService.GetById(newProduct.CountryId);

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
