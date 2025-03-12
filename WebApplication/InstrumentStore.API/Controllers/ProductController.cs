using AutoMapper;
using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.Contracts.Filters;
using InstrumentStore.Domain.Contracts.Products;
using InstrumentStore.Domain.DataBase;
using InstrumentStore.Domain.DataBase.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace InstrumentStore.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ProductController : ControllerBase
	{
		private readonly InstrumentStoreDBContext _dbContext;
		private readonly IProductService _productService;
		private readonly IImageService _imageService;
		private readonly IProductPropertyService _productPropertyService;
		private readonly IMapper _mapper;

		public ProductController(IProductService productService,
			IProductPropertyService productPropertyService,
			IMapper mapper,
			InstrumentStoreDBContext dbContext,
			IImageService imageService)
		{
			_productService = productService;
			_productPropertyService = productPropertyService;
			_mapper = mapper;
			_dbContext = dbContext;
			_imageService = imageService;
		}

		[HttpGet("page{page}")]
		public async Task<ActionResult<List<ProductCard>>> GetAllProductsCards([FromRoute] int page)
		{
			List<Product> products = await _productService.GetAll(page);
			products = products.Where(p => p.IsArchive == false).ToList();
			List<ProductCard> productsCards = new List<ProductCard>();

			foreach (var p in products)
				productsCards.Add(_mapper.Map<ProductCard>(p, opt => opt.Items["DbContext"] = _dbContext));

			return Ok(productsCards);
		}

		[HttpGet("category/{category}/page{page}")]// функция получения товаров с фильтрацией
		public async Task<ActionResult<List<ProductCard>>> GetAllProductsByCategoryWithFilters(
			[FromRoute] string category,
			[FromRoute] int page,
			[FromQuery] string? filters)
		{// получени товаров выбранной котегории
			List<Product> products = await _productService.GetAllByCategory(category, page);
			products = products.Where(p => p.IsArchive == false).ToList();

			FilterRequest filterRequest = null;
			if (!string.IsNullOrEmpty(filters))
			{// фильтрация если она выбранна
				filterRequest = JsonConvert.DeserializeObject<FilterRequest>(filters);

				products = await _productService.GetAllWithFilters(category, filterRequest, products, page);
			}

			List<ProductCard> productsCards = new List<ProductCard>();

			foreach (var p in products)
				productsCards.Add(_mapper.Map<ProductCard>(p, opt => opt.Items["DbContext"] = _dbContext));

			return Ok(productsCards);
		}

		[HttpGet("category-filters/{category}")]
		public async Task<ActionResult> GetCategoryFilters([FromRoute] string category)
		{
			return Ok(await _productPropertyService.GetCategoryFilters(category));
		}

		[HttpGet("search/page{page}")]// функция поиска товаров по имени
		public async Task<ActionResult<List<ProductCard>>> SearchByName(
			[FromRoute] int page,
			[FromQuery] string? name)
		{
			List<ProductCard> productsCards = await _productService.SearchByName(name, page);
			productsCards = productsCards.Where(p => p.IsArchive == false).ToList();

			return Ok(productsCards);
		}

		[Authorize(Roles = "admin")]
		[HttpGet("search-with-archive/page{page}")]// функция поиска товаров по имени включая архивные товары
		public async Task<ActionResult<List<ProductCard>>> SearchByNameWithArchive(
			[FromRoute] int page,
			[FromQuery] string? name)
		{
			var products = await _productService.SearchByName(name, page);

			List<ProductCard> productsCards = new List<ProductCard>();

			foreach (var p in products)
				productsCards.Add(_mapper.Map<ProductCard>(p, opt => opt.Items["DbContext"] = _dbContext));

			return Ok(productsCards);
		}

		[HttpGet("{id:guid}")]
		public async Task<ActionResult<ProductResponse>> GetProduct([FromRoute] Guid id)
		{
			var product = await _productService.GetById(id);
			var productResponseData = _mapper.Map<ProductData>(product, opt => opt.Items["DbContext"] = _dbContext);

			ProductResponse response = new ProductResponse(
				productResponseData,
				await _productPropertyService.GetProductProperties(id));

			return Ok(response);
		}

		[HttpPut("update-product/{id:guid}")]
		public async Task<ActionResult<Guid>> UpdateProduct(
			Guid id,
			[FromForm] string productDto,
			[FromForm] List<IFormFile> images)
		{
			var productRequest = JsonConvert
				.DeserializeObject<CreateProductRequest>(productDto);

			if (images != null && images.Any())
			{
				foreach (var image in images)
				{
					if (await _imageService.IsImage(image) == false)
						throw new BadImageFormatException(
							$"Format {image.ContentType} is not available");
				}
			}

			return Ok(await _productService.Update(id, productRequest, images));
		}

		[HttpGet("get-product-to-update/{id:guid}")]
		public async Task<ActionResult<ProductToUpdateResponse>> GetProductToUpdate(Guid id)
		{
			Product product = await _productService.GetById(id);

			if (product.IsArchive)
				throw new ArgumentException();

			return Ok(_mapper.Map<ProductToUpdateResponse>(product,
				opt => opt.Items["DbContext"] = _dbContext));
		}

		[HttpDelete("{id:guid}")]
		public async Task<ActionResult<Guid>> DeleteProduct(Guid id)
		{
			return Ok(await _productService.Delete(id));
		}

		[HttpPost("create-product")]
		public async Task<ActionResult<Guid>> CreateProduct(
			[FromForm] string productDto,
			[FromForm] List<IFormFile> images)
		{
			var productRequest = JsonConvert
				.DeserializeObject<CreateProductRequest>(productDto);

			if (images != null && images.Any())
			{
				foreach (var image in images)
				{
					if (await _imageService.IsImage(image) == false)
						throw new BadImageFormatException(
							$"Format {image.ContentType} is not available");
				}
			}

			return Ok(await _productService.Create(productRequest, images));
		}

		[HttpGet("get-products-for-admin{page}")]
		public async Task<ActionResult<List<ProductData>>> GetProductsForAdmin([FromRoute] int page)
		{
			List<Product> products = await _productService.GetAll(1);
			List<ProductData> productsDatas = new List<ProductData>();

			foreach (var p in products)
			{
				productsDatas.Add(_mapper.Map<ProductData>(p, opt => opt.Items["DbContext"] = _dbContext));
				productsDatas.Last().Name += page;
			}

			productsDatas.AddRange(productsDatas);
			productsDatas.AddRange(productsDatas);
			productsDatas.AddRange(productsDatas);
			productsDatas.AddRange(productsDatas);

			Console.WriteLine("\n" + page + "\n" + productsDatas.Count);
			productsDatas = productsDatas
				.Skip((page - 1) * IProductService.pageSize)
				.Take(IProductService.pageSize)
				.ToList();
			Console.WriteLine(productsDatas.Count);

			return Ok(productsDatas);
		}
	}
}
