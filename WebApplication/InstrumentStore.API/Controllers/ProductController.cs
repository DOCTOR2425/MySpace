using AutoMapper;
using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.Contracts.Comment;
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
		private readonly ICommentService _commentService;
		private readonly IUserService _usersService;
		private readonly IMapper _mapper;

		public ProductController(IProductService productService,
			IProductPropertyService productPropertyService,
			IMapper mapper,
			InstrumentStoreDBContext dbContext,
			IImageService imageService,
			ICommentService commentService,
			IUserService usersService)
		{
			_productService = productService;
			_productPropertyService = productPropertyService;
			_mapper = mapper;
			_dbContext = dbContext;
			_imageService = imageService;
			_commentService = commentService;
			_usersService = usersService;
		}

		private async Task<Guid?> GetUserIdFromToken()
		{
			string? token = HttpContext.Request.Headers["Authorization"];

			if (token == null)
				return null;

			return (await _usersService.GetUserFromToken(HttpContext.Request.Headers["Authorization"]
					.ToString().Substring("Bearer ".Length).Trim())).UserId;
		}

		[Authorize]
		[HttpGet("get-special-products-for-user")]
		public async Task<ActionResult<List<UserProductCard>>> GetSpecialProductsForUser()
		{
			Guid userId = (Guid)await GetUserIdFromToken();

			List<Product> products = await _productService.GetSpecialProductsForUser(userId);
			return Ok(await _productService.GetUserProductCards(products, userId));

		}

		[HttpGet("category/{categoryId:guid}/page{page}")]
		public async Task<IActionResult> GetAllProductsByCategoryWithFilters(
			[FromRoute] Guid categoryId,
			[FromRoute] int page,
			[FromQuery] string? filters)
		{
			List<Product> products = await _productService.GetAllByCategory(categoryId);
			products = products.Where(p => p.IsArchive == false).ToList();

			if (!string.IsNullOrEmpty(filters))
			{
				FilterRequest filterRequest = JsonConvert.DeserializeObject<FilterRequest>(filters);
				products = await _productService.GetAllWithFilters(categoryId, filterRequest, products);
			}

			List<UserProductCard> cards = await _productService.GetUserProductCards(products
						.Skip((page - 1) * IProductService.PageSize)
						.Take(IProductService.PageSize)
						.ToList(),
					await GetUserIdFromToken());

			return Ok(new
			{
				items = cards,
				totalCount = products.Count
			});
		}

		[HttpGet("category-filters/{categoryId:guid}")]
		public async Task<IActionResult> GetCategoryFilters([FromRoute] Guid categoryId)
		{
			return Ok(await _productPropertyService.GetCategoryFilters(categoryId));
		}

		[HttpGet("search/page{page}")]
		public async Task<ActionResult<List<UserProductCard>>> SearchByName(
			[FromRoute] int page,
			[FromQuery] string? name)
		{
			List<UserProductCard> productsCards = _mapper.Map<List<UserProductCard>>(
				await _productService.SearchByName(name, page));

			productsCards = productsCards.Where(p => p.IsArchive == false).ToList();
			foreach (var p in productsCards)
				p.Image = "https://localhost:7295/images/" + p.Image;

			return Ok(productsCards);
		}

		[Authorize(Roles = "admin")]
		[HttpGet("search-with-archive/page{page}")]
		public async Task<ActionResult<List<AdminProductCard>>> SearchByNameWithArchive(
			[FromRoute] int page,
			[FromQuery] string name = "")
		{
			List<AdminProductCard> products = await _productService.SearchByName(name, page);

			for (int i = 0; i < products.Count; i++)
				products[i].Image = "https://localhost:7295/images/" + products[i].Image;

			return Ok(products);
		}

		[HttpGet("get-product/{id:guid}")]
		public async Task<ActionResult<FullProductInfoResponse>> GetProduct([FromRoute] Guid id)
		{
			return Ok(await _productService.GetFullProductInfoResponse(id));
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

		[Authorize(Roles = "admin")]
		[HttpPut("change-archive-status/{id:guid}")]
		public async Task<ActionResult<Guid>> ChangeArchiveStatus(
			Guid id,
			[FromQuery] bool archiveStatus)
		{
			return Ok(await _productService.ChangeArchiveStatus(id, archiveStatus));
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
		public async Task<ActionResult<List<AdminProductCard>>> GetProductsForAdmin([FromRoute] int page)
		{
			return Ok(_mapper.Map<List<AdminProductCard>>(
				await _productService.GetAll(page), opt => opt.Items["DbContext"] = _dbContext));
		}

		[Authorize]
		[HttpPost("add-comment")]
		public async Task<IActionResult> AddComment([FromBody] CreateCommentRequest request)
		{
			return Ok(await _commentService.CreateCommentToProduct(
				request.Text,
				request.ProductId,
				(await _usersService.GetUserFromToken(HttpContext.Request.Headers["Authorization"]
				.ToString().Substring("Bearer ".Length).Trim())).UserId));
		}

		[HttpGet("get-comments-by-product/{id:guid}")]
		public async Task<IActionResult> GetCommentsByProduct(Guid id)
		{
			return Ok(_mapper.Map<List<CommentResponse>>(
				await _commentService.GetAllCommentsByProduct(id)));
		}

		[HttpGet("get-simmular-to-product/{productId:guid}")]
		public async Task<ActionResult<List<UserProductCard>>> GetSimmularToProduct(
			[FromRoute] Guid productId)
		{
			return Ok((await _productService.GetUserProductCards(
					await _productService.GetSimmularToProduct(productId),
					await GetUserIdFromToken()))
				.Take(10));
		}

		[HttpGet("get-most-popular-products/page{page}")]
		public async Task<IActionResult> GetMostPopularProducts([FromRoute] int page)
		{
			return Ok(await _productService.GetUserProductCards(
				await _productService.GetProductsByPopularity(page),
				await GetUserIdFromToken()));
		}

		[HttpGet("get-product-minimal-data/{productId:guid}")]
		public async Task<ActionResult<ProductMinimalData>> GetProductMinimalData(
			[FromRoute] Guid productId)
		{
			return Ok(await _productService.GetProductMinimalData(productId));
		}
	}
}
