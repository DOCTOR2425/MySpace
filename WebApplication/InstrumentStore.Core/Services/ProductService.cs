using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.Contracts.Filters;
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
        private readonly IProductCategoryService _productCategoryService;

        public ProductService(InstrumentStoreDBContext dbContext,
            IBrandService brandService,
            ICountryService countryService,
            IProductCategoryService productCategoryService)
        {
            _dbContext = dbContext;
            _brandService = brandService;
            _countryService = countryService;
            _productCategoryService = productCategoryService;
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

        public async Task<List<Product>> GetAllByCategory(string categoryName)
        {
            Guid categoryId = (await _dbContext.ProductCategory
                .FirstOrDefaultAsync(c => c.Name.ToLower() == categoryName.ToLower()))
                    .ProductCategoryId;

            return await _dbContext.Product
                .Include(p => p.ProductCategory)
                .Include(p => p.Brand)
                .Include(p => p.Country)
                .Where(p => p.ProductCategory.ProductCategoryId == categoryId)
                .ToListAsync();
        }

        public async Task<List<Product>> GetAllWithFilters(FilterRequest filter, List<Product> productsForFilter)
        {
            List<Product> products = await FilterByStaticProperties(filter, productsForFilter);

            products = await FilterByUnStaticProperties(filter, products);

            return products;
        }

        private async Task<List<Product>> FilterByUnStaticProperties(FilterRequest filter, List<Product> productsForFilter)
        {
            return productsForFilter;
        }

        private async Task<List<Product>> FilterByStaticProperties(FilterRequest filter, List<Product> productsForFilter)
        {
            List<Product> products = await FilterByPrice(filter, productsForFilter);
            products = await FilterByBrand(filter, products);
            products = await FilterByCountry(filter, products);

            return products;
        }

        private async Task<List<Product>> FilterByPrice(FilterRequest filter, List<Product> productsForFilter)
        {
            RangeFilter? priceRange = filter.RangeFilters
                .FirstOrDefault(f => f.Property.ToLower() == "price");

            if (priceRange == null)
                return productsForFilter;

            List<Product> products = new List<Product>();
            foreach (var product in productsForFilter)
                if (product.Price >= priceRange.ValueFrom &&
                        product.Price <= priceRange.ValueTo)
                {
                    products.Add(product);
                    Console.WriteLine(product.Price);
                }

            return products;
        }

        private async Task<List<Product>> FilterByBrand(FilterRequest filter, List<Product> productsForFilter)
        {
            List<CollectionFilter> collectionFilters = filter.CollectionFilters
                .Where(f => f.PropertyId.ToLower() == "brand")
                .ToList();

            if (collectionFilters.Count == 0)
                return productsForFilter;

            List<Product> productsAfterFilter = new List<Product>();

            foreach (var f in collectionFilters)
                foreach (var p in productsForFilter)
                    if (p.Brand.Name.ToLower() == f.PropertyValue.ToLower())
                        productsAfterFilter.Add(p);

            return productsAfterFilter;
        }

        private async Task<List<Product>> FilterByCountry(FilterRequest filter, List<Product> productsForFilter)
        {
            List<CollectionFilter> collectionFilters = filter.CollectionFilters
                .Where(f => f.PropertyId.ToLower() == "country")
                .ToList();

            if (collectionFilters.Count == 0)
                return productsForFilter;

            List<Product> productsAfterFilter = new List<Product>();

            foreach (var f in collectionFilters)
                foreach (var p in productsForFilter)
                    if (p.Country.Name.ToLower() == f.PropertyValue.ToLower())
                        productsAfterFilter.Add(p);

            return productsAfterFilter;
        }

        public async Task<Product> GetById(Guid id)
        {
            return await _dbContext.Product
                .Include(p => p.ProductCategory)
                .Include(p => p.Brand)
                .Include(p => p.Country)
                .FirstOrDefaultAsync(p => p.ProductId == id);
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

                ProductCategory = await _productCategoryService.GetById(productRequest.ProductTypeId),
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
            product.ProductCategory = await _productCategoryService.GetById(newProduct.ProductTypeId);
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
