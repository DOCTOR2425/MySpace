using AutoMapper;
using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.Contracts.Filters;
using InstrumentStore.Domain.Contracts.ProductProperties;
using InstrumentStore.Domain.Contracts.Products;
using InstrumentStore.Domain.DataBase;
using InstrumentStore.Domain.DataBase.Models;
using Microsoft.AspNetCore.Http;
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
		private readonly IPaidOrderService _paidOrderService;
		private readonly IImageService _imageService;
		private readonly IMapper _mapper;

		public ProductService(InstrumentStoreDBContext dbContext,
			IBrandService brandService,
			ICountryService countryService,
			IProductCategoryService productCategoryService,
			IProductPropertyService productPropertyService,
			IProductFilterService productFilterService,
			IImageService imageService,
			IPaidOrderService paidOrderService,
			IMapper mapper)
		{
			_dbContext = dbContext;
			_brandService = brandService;
			_countryService = countryService;
			_productCategoryService = productCategoryService;
			_productPropertyService = productPropertyService;
			_productFilterService = productFilterService;
			_imageService = imageService;
			_paidOrderService = paidOrderService;
			_mapper = mapper;
		}

		public async Task<List<Product>> GetAll(int page)
		{
			return await _dbContext.Product
				.Include(p => p.ProductCategory)
				.Include(p => p.Brand)
				.Include(p => p.Country)
				.OrderBy(p => p.Name)
				.Skip((page - 1) * IProductService.PageSize)
				.Take(IProductService.PageSize)
				.AsNoTracking()
				.ToListAsync();
		}

		public async Task<List<Product>> GetAllByCategory(Guid categoryId)
		{
			return await _dbContext.Product
				.Include(p => p.ProductCategory)
				.Include(p => p.Brand)
				.Include(p => p.Country)
				.OrderBy(p => p.Name)
				.Where(p => p.ProductCategory.ProductCategoryId == categoryId)
				.ToListAsync();
		}

		public async Task<List<ProductCard>> SearchByName(string input, int page)
		{
			return await _dbContext.Set<ProductCard>()
					.FromSqlRaw("EXEC SearchByName @p0, @p1", input, page).ToListAsync();
		}

		public async Task<List<Product>> GetAllWithFilters(
			Guid categoryId,
			FilterRequest filter,
			List<Product> productsForFilter)
		{
			return await _productFilterService.GetAllWithFilters(categoryId, filter, productsForFilter);
		}

		public async Task<Product> GetById(Guid id)
		{
			return await _dbContext.Product
				.Include(p => p.ProductCategory)
				.Include(p => p.Brand)
				.Include(p => p.Country)
				.FirstOrDefaultAsync(p => p.ProductId == id);
		}

		public async Task<FullProductInfoResponse> GetFullProductInfoResponse(Guid productId)
		{
			Product product = await _dbContext.Product
				.Include(p => p.ProductCategory)
				.Include(p => p.Brand)
				.Include(p => p.Country)
				.FirstOrDefaultAsync(p => p.ProductId == productId);

			FullProductInfoResponse response = _mapper.Map<FullProductInfoResponse>(product);

			List<string> images = (await _imageService.GetByProductId(productId))
						.Select(i => i.Name)
						.ToList();

			foreach (var image in images)
				response.Images.Add("https://localhost:7295/images/" + image);

			List<ProductPropertyValue> propertyValues = _dbContext.ProductPropertyValue
				.Include(p => p.ProductProperty)
				.Where(p => p.Product.ProductId == product.ProductId)
				.ToList();

			foreach (var propertyValue in propertyValues)
				response.ProductPropertyValues
					.Add(_mapper.Map<ProductPropertyValuesResponse>(propertyValue));

			return response;
		}

		public async Task<Guid> Create(Product product)
		{
			await _dbContext.Product.AddAsync(product);
			await _dbContext.SaveChangesAsync();

			return product.ProductId;
		}

		public async Task<Guid> Create(CreateProductRequest productRequest, List<IFormFile> images)
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
			await _dbContext.Product.AddAsync(product);
			await _dbContext.SaveChangesAsync();

			await SaveImagesToProduct(product, images);
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
			List<IFormFile> images)
		{
			FileStream stream;
			string filePath = "";
			for (int i = 0; i < images.Count; i++)
			{
				await _imageService.Create(new Image()
				{
					ImageId = Guid.NewGuid(),
					Product = product,
					Index = i,
					Name = images[i].FileName
				});

				filePath = Path.Combine("wwwroot/images", images[i].FileName);
				using (stream = new FileStream(filePath, FileMode.Create))
				{
					await images[i].CopyToAsync(stream);
				};
			}
		}

		public async Task<Guid> Update(
			Guid oldId,
			CreateProductRequest productRequest,
			List<IFormFile> images)
		{
			Product product = await GetById(oldId);

			product.Name = productRequest.Name;
			product.Description = productRequest.Description;
			product.Price = productRequest.Price;
			product.Quantity = productRequest.Quantity;
			product.ProductCategory = await _productCategoryService.GetById(productRequest.ProductCategoryId);
			product.Brand = await _brandService.GetById(productRequest.BrandId);
			product.Country = await _countryService.GetById(productRequest.CountryId);

			await _dbContext.SaveChangesAsync();

			await _productPropertyService.DeleteProperiesValuesByProductId(oldId);
			await _imageService.DeleteImagesByProductId(oldId);
			await SaveImagesToProduct(product, images);
			await SaveProductPropertiesValues(product, productRequest);

			return oldId;
		}

		public async Task<Guid> ChangeArchiveStatus(Guid id, bool archiveStatus)
		{
			(await _dbContext.Product
				.FindAsync(id)).IsArchive = archiveStatus;

			_dbContext.SaveChanges();

			return id;
		}

		public async Task<List<Product>> GetSimmularToProduct(Guid productId)
		{
			Product? target = await _dbContext.Product
				.Include(p => p.ProductCategory)
				.FirstOrDefaultAsync(p => p.ProductId == productId);

			if (target == null)
				throw new ArgumentNullException("Такого товара не существует");

			List<Product> productsInCategoryOrderedBySales = await _dbContext.ProductCategory
				.Where(category => category.ProductCategoryId == target.ProductCategory.ProductCategoryId)
				.SelectMany(category => _dbContext.PaidOrderItem
					.Include(oi => oi.Product)
					.Include(oi => oi.Product.ProductCategory)
					.Where(oi => oi.Product.ProductCategory.ProductCategoryId == category.ProductCategoryId)
					.GroupBy(oi => oi.Product)
					.Select(g => new
					{
						Product = g.Key,
						TotalSales = g.Sum(oi => oi.Quantity)
					})
					.OrderByDescending(x => x.TotalSales)
					.Select(x => x.Product)
				)
				.ToListAsync();
			productsInCategoryOrderedBySales.Remove(target);

			return productsInCategoryOrderedBySales;
		}

		public async Task<List<Product>> GetSpecialProductsForUser(Guid userId)
		{
			Dictionary<ProductCategory, int> categoryPoints = new Dictionary<ProductCategory, int>();

			await GetPaidOrderItemPoints(userId, categoryPoints);
			await GetCartItemPoints(userId, categoryPoints);
			await GetComparisonItemPoints(userId, categoryPoints);

			List<Product> products = new List<Product>();
			int pointsAmount = categoryPoints.Sum(c => c.Value);
			foreach (var category in categoryPoints)
			{
				products.AddRange(_dbContext.Product
					.Where(p => p.ProductCategory == category.Key)
					.Take((int)Math.Round((decimal)(category.Value * IProductService.PageSize) / pointsAmount))
					.ToList());
			}

			int missingItems = IProductService.PageSize - products.Count;
			if (missingItems > 0)
				products = products.Union(await GetProductsByPopularity(1)).ToList();

			return products.Take(IProductService.PageSize).ToList();
		}

		private async Task GetPaidOrderItemPoints(Guid userId, Dictionary<ProductCategory, int> categoryPoints)
		{
			foreach (var order in await _paidOrderService.GetAllByUserId(userId))
				foreach (var item in await _paidOrderService.GetAllItemsByOrder(order.PaidOrderId))
				{
					try
					{
						categoryPoints.Add(item.Product.ProductCategory, 1);
					}
					catch (Exception e)
					{
						categoryPoints[item.Product.ProductCategory] += 1;
					}
				}
		}

		private async Task GetCartItemPoints(Guid userId, Dictionary<ProductCategory, int> categoryPoints)
		{
			foreach (var item in await _dbContext.CartItem
				.Include(i => i.Product)
				.Include(i => i.Product.ProductCategory)
				.Where(i => i.User.UserId == userId)
				.ToListAsync())
			{
				try
				{
					categoryPoints.Add(item.Product.ProductCategory, 1);
				}
				catch (Exception e)
				{
					categoryPoints[item.Product.ProductCategory] += 1;
				}
			}
		}

		private async Task GetComparisonItemPoints(Guid userId, Dictionary<ProductCategory, int> categoryPoints)
		{
			foreach (var item in await _dbContext.ProductComparisonItem
				.Include(i => i.Product.ProductCategory)
				.Where(i => i.User.UserId == userId)
				.ToListAsync())
			{
				try
				{
					categoryPoints.Add(item.Product.ProductCategory, 1);
				}
				catch (Exception e)
				{
					categoryPoints[item.Product.ProductCategory] += 1;
				}
			}
		}

		public async Task<List<Product>> GetProductsByPopularity(int page)
		{
			var paidOrders = await _dbContext.PaidOrderItem
				.GroupBy(item => item.Product.ProductId)
				.Select(g => new { ProductId = g.Key, Count = g.Count() })
				.ToListAsync();

			var cartItems = await _dbContext.CartItem
				.GroupBy(item => item.Product.ProductId)
				.Select(g => new { ProductId = g.Key, Count = g.Count() })
				.ToListAsync();

			var comparisons = await _dbContext.ProductComparisonItem
				.GroupBy(item => item.Product.ProductId)
				.Select(g => new { ProductId = g.Key, Count = g.Count() })
				.ToListAsync();

			var paidOrdersDict = paidOrders.ToDictionary(x => x.ProductId, x => x.Count);
			var cartItemsDict = cartItems.ToDictionary(x => x.ProductId, x => x.Count);
			var comparisonsDict = comparisons.ToDictionary(x => x.ProductId, x => x.Count);

			var allProductIds = paidOrdersDict.Keys
				.Union(cartItemsDict.Keys)
				.Union(comparisonsDict.Keys)
				.ToList();

			var products = await _dbContext.Product
				.Where(p => allProductIds.Contains(p.ProductId))
				.Include(p => p.Brand)
				.Include(p => p.Country)
				.Include(p => p.ProductCategory)
				.ToListAsync();

			var sortedProducts = products
				.Select(p => new
				{
					Product = p,
					TotalPopularity =
						(paidOrdersDict.TryGetValue(p.ProductId, out var poCount) ? poCount : 0) +
						(cartItemsDict.TryGetValue(p.ProductId, out var ciCount) ? ciCount : 0) +
						(comparisonsDict.TryGetValue(p.ProductId, out var cCount) ? cCount : 0)
				})
				.OrderByDescending(x => x.TotalPopularity)
				.Skip((page - 1) * IProductService.PageSize)
				.Take(IProductService.PageSize)
				.Select(x => x.Product)
				.ToList();

			return sortedProducts;
		}

		public async Task<ProductMinimalData> GetProductMinimalData(Guid productId)
		{
			Product product = await GetById(productId);
			ProductMinimalData response = _mapper.Map<ProductMinimalData>(product);
			response.Image = "https://localhost:7295/images/" +
				(await _imageService.GetMainProductImage(productId)).Name;

			return response;
		}
	}
}
