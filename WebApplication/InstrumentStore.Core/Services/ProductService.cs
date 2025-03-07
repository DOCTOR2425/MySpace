using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.Contracts.Filters;
using InstrumentStore.Domain.Contracts.Products;
using InstrumentStore.Domain.DataBase;
using InstrumentStore.Domain.DataBase.Models;
using InstrumentStore.Domain.DataBase.ProcedureResultModels;
using Microsoft.EntityFrameworkCore;

namespace InstrumentStore.Domain.Service
{
	public class ProductService : IProductService
	{
		private readonly InstrumentStoreDBContext _dbContext;
		private readonly IBrandService _brandService;
		private readonly ICountryService _countryService;
		private readonly IProductCategoryService _productCategoryService;
		private readonly IProductPropertyService _productPropertyService;
		private readonly IProductFilterService _productFilterService;
		private readonly IImageService _imageService;
		public const int pageSize = 10;

		public ProductService(InstrumentStoreDBContext dbContext,
			IBrandService brandService,
			ICountryService countryService,
			IProductCategoryService productCategoryService,
			IProductPropertyService productPropertyService,
			IProductFilterService productFilterService,
			IImageService imageService)
		{
			_dbContext = dbContext;
			_brandService = brandService;
			_countryService = countryService;
			_productCategoryService = productCategoryService;
			_productPropertyService = productPropertyService;
			_productFilterService = productFilterService;
			_imageService = imageService;
		}

		public async Task<List<Product>> GetAll(int page)
		{
			return await _dbContext.Product
				.Include(p => p.ProductCategory)
				.Include(p => p.Brand)
				.Include(p => p.Country)
				.OrderBy(p => p.Name)
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.AsNoTracking()
				.ToListAsync();
		}

		public async Task<List<Product>> GetAllByCategory(string categoryName, int page)
		{
			Guid? categoryId = (await _dbContext.ProductCategory
				.FirstOrDefaultAsync(c => c.Name.ToLower() == categoryName.ToLower()))
					.ProductCategoryId;

			return await _dbContext.Product
				.Include(p => p.ProductCategory)
				.Include(p => p.Brand)
				.Include(p => p.Country)
				.OrderBy(p => p.Name)
				.Where(p => p.ProductCategory.ProductCategoryId == categoryId)
				.ToListAsync();
		}

		public async Task<List<Product>> SearchByName(string input, int page)
		{
			List<ProductSearchResult> list = _dbContext.Set<ProductSearchResult>()
					.FromSqlRaw("EXEC SearchByName @p0, @p1", input, page).ToList();

			var products = list.Select(item => new Product
			{
				ProductId = item.ProductId,
				Description = item.Description,
				Name = item.Name,
				Price = item.Price,
				Quantity = item.Quantity,
				ProductCategory = new ProductCategory
				{
					ProductCategoryId = item.ProductCategoryId2,
					Name = item.ProductCategoryName
				},
				Brand = new Brand
				{
					BrandId = item.BrandId2,
					Name = item.BrandName
				},
				Country = new Country
				{
					CountryId = item.CountryId2,
					Name = item.CountryName
				}
			}).ToList();

			return products;
		}

		public async Task<List<Product>> GetAllWithFilters(
			string categoryName,
			FilterRequest filter,
			List<Product> productsForFilter,
			int page)
		{
			return await _productFilterService.GetAllWithFilters(categoryName, filter, productsForFilter, page);
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

		public async Task<Guid> Create(CreateProductRequest productRequest)
		{
			Product product = new Product
			{
				ProductId = Guid.NewGuid(),
				Name = productRequest.Name,
				Description = productRequest.Description,
				Price = productRequest.Price,
				Quantity = productRequest.Quantity,

				ProductCategory = await _productCategoryService.GetById(productRequest.ProductCategoryId),
				Brand = await _brandService.GetById(productRequest.BrandId),
				Country = await _countryService.GetById(productRequest.CountryId)
			};
			_dbContext.Product.AddAsync(product);
			_dbContext.SaveChangesAsync();

			await SaveImagesToProduct(product, productRequest);
			await SaveProductPropertiesValues(product, productRequest);

			return product.ProductId;
		}

		private async Task SaveProductPropertiesValues(
			Product product,
            CreateProductRequest productRequest)
		{
			foreach (Guid propertyId in productRequest.PropertyValues.Keys)
			{
				ProductPropertyValue propertyValue = new ProductPropertyValue()
				{
					ProductPropertyValueId = Guid.NewGuid(),
					Product = product,
					ProductProperty = await _productPropertyService.GetById(propertyId),
					Value = productRequest.PropertyValues[propertyId]
                };
				await _productPropertyService.CreatePropertyValue(propertyValue);
			}
		}

		private async Task SaveImagesToProduct(
			Product product,
			CreateProductRequest productRequest)
		{
			FileStream stream;
			string filePath = "";
			foreach (var image in productRequest.Images)
			{
				_imageService.Create(new Image()
				{
					ImageId = Guid.NewGuid(),
					Product = product,
					Name = image.FileName
				});

				filePath = Path.Combine("wwwroot/images", image.FileName);
				using (stream = new FileStream(filePath, FileMode.Create))
				{
					await image.CopyToAsync(stream);
				};
			}
		}

		public async Task<Guid> Update(Guid oldId, Product newProduct)
		{
			Product product = await _dbContext.Product.FindAsync(oldId);

			product.Name = newProduct.Name;
			product.ProductCategory = newProduct.ProductCategory;
			product.Brand = newProduct.Brand;
			product.Country = newProduct.Country;
			product.Description = newProduct.Description;
			product.Price = newProduct.Price;
			product.Quantity = newProduct.Quantity;

			await _dbContext.SaveChangesAsync();

			return oldId;
		}

		public async Task<Guid> Update(Guid oldId, CreateProductRequest newProduct)
		{
			Product product = await GetById(oldId);

			product.Name = newProduct.Name;
			product.Description = newProduct.Description;
			product.Price = newProduct.Price;
			product.Quantity = newProduct.Quantity;
			product.ProductCategory = await _productCategoryService.GetById(newProduct.ProductCategoryId);
			product.Brand = await _brandService.GetById(newProduct.BrandId);
			product.Country = await _countryService.GetById(newProduct.CountryId);

			await _dbContext.SaveChangesAsync();

			return oldId;
		}

		public async Task<Guid> Delete(Guid id)
		{
			await _dbContext.Product
				.Where(p => p.ProductId == id)//триггер написать надо
				.ExecuteDeleteAsync();

			return id;
		}
	}
}
