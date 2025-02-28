using AutoMapper;
using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.Contracts.Filters;
using InstrumentStore.Domain.Contracts.Products;
using InstrumentStore.Domain.DataBase;
using InstrumentStore.Domain.DataBase.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace InstrumentStore.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ProductController : ControllerBase
	{
		private readonly InstrumentStoreDBContext _dbContext;
		private readonly IProductService _productService;
		private readonly IProductPropertyService _productPropertyService;
		private readonly IImageService _imageService;
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
			[FromQuery] string? input)
		{
			Console.WriteLine(input);
			var products = await _productService.SearchByName(input, page);

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

		/*
		{
  "name": "string",
  "description": "string",
  "price": 10,
  "quantity": 10,
  "images": [
	"D:\модели и рисунки\КисаСЖемчужиной.jpg"
  ],
  "productCategoryId": "AD93A450-E2B4-4DB0-BB0B-37864E96D00A",
  "brandId": "76FCE919-BADD-4E2B-816B-89D60D325181",
  "countryId": "CF70DB7F-B55B-414B-A724-A9820C938CE5"
}
		*/

		[HttpPost]
		public async Task<ActionResult<Guid>> CreateProduct(
			[FromForm] ProductRequest productRequest)
		{
			if (productRequest.Images != null && productRequest.Images.Any())
			{
				foreach (var image in productRequest.Images)
				{
					if (await _imageService.IsImage(image) == false)
						throw new BadImageFormatException(
							$"Format {image.ContentType} is not available");
				}
			}

			return Ok(await _productService.Create(productRequest));
		}

		[HttpPut("{id:guid}")]
		public async Task<ActionResult<Guid>> UpdateProduct(Guid id, [FromBody] ProductRequest productRequest)
		{
			return Ok(await _productService.Update(id, productRequest));
		}

		[HttpDelete("{id:guid}")]
		public async Task<ActionResult<Guid>> DeleteProduct(Guid id)
		{
			return Ok(await _productService.Delete(id));
		}
	}
}
