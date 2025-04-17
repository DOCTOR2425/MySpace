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
        private readonly IUsersService _usersService;
        private readonly IMapper _mapper;

        public ProductController(IProductService productService,
            IProductPropertyService productPropertyService,
            IMapper mapper,
            InstrumentStoreDBContext dbContext,
            IImageService imageService,
            ICommentService commentService,
            IUsersService usersService)
        {
            _productService = productService;
            _productPropertyService = productPropertyService;
            _mapper = mapper;
            _dbContext = dbContext;
            _imageService = imageService;
            _commentService = commentService;
            _usersService = usersService;
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

        [Authorize]
        [HttpGet("get-special-products-for-user")]
        public async Task<ActionResult<List<ProductCard>>> GetSpecialProductsForUser()
        {
            List<Product> products = await _productService.GetSpecialProductsForUser(
                (await _usersService.GetUserFromToken(HttpContext.Request.Headers["Authorization"]
                    .ToString().Substring("Bearer ".Length).Trim())).UserId);

            List<ProductCard> productsCards = new List<ProductCard>();
            foreach (var p in products)
                productsCards.Add(_mapper.Map<ProductCard>(p, opt => opt.Items["DbContext"] = _dbContext));

            return Ok(productsCards);

        }

        [HttpGet("category/{categoryId:guid}/page{page}")]// функция получения товаров с фильтрацией
        public async Task<IActionResult> GetAllProductsByCategoryWithFilters(
            [FromRoute] Guid categoryId,
            [FromRoute] int page,
            [FromQuery] string? filters)
        {// получени товаров выбранной котегории
            List<Product> products = await _productService.GetAllByCategory(categoryId);
            products = products.Where(p => p.IsArchive == false).ToList();

            if (!string.IsNullOrEmpty(filters))
            {// фильтрация если она выбранна
                FilterRequest filterRequest = JsonConvert.DeserializeObject<FilterRequest>(filters);
                products = await _productService.GetAllWithFilters(categoryId, filterRequest, products);
            }

            List<ProductCard> cards = _mapper.Map<List<ProductCard>>(products
                    .Skip((page - 1) * IProductService.PageSize)
                    .Take(IProductService.PageSize),
                opt => opt.Items["DbContext"] = _dbContext);

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

        [HttpGet("search/page{page}")]// функция поиска товаров по имени
        public async Task<ActionResult<List<ProductCard>>> SearchByName(
            [FromRoute] int page,
            [FromQuery] string? name)
        {
            List<ProductCard> productsCards = await _productService.SearchByName(name, page);
            productsCards = productsCards.Where(p => p.IsArchive == false).ToList();
            foreach (var p in productsCards)
                p.Image = "https://localhost:7295/images/" + p.Image;

            return Ok(productsCards);
        }

        [Authorize(Roles = "admin")]
        [HttpGet("search-with-archive/page{page}")]// функция поиска товаров по имени включая архивные товары
        public async Task<ActionResult<List<ProductCard>>> SearchByNameWithArchive(
            [FromRoute] int page,
            [FromQuery] string name = "")
        {
            List<ProductCard> products = await _productService.SearchByName(name, page);

            for (int i = 0; i < products.Count; i++)
                products[i].Image = "https://localhost:7295/images/" + products[i].Image;

            return Ok(products);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<FullProductInfoResponse>> GetProduct([FromRoute] Guid id)
        {
            var product = await _productService.GetById(id);
            FullProductInfoResponse productResponseData = _mapper
                .Map<FullProductInfoResponse>(product, opt => opt.Items["DbContext"] = _dbContext);

            return Ok(productResponseData);
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
        public async Task<ActionResult<FullProductInfoResponse>> GetProductToUpdate(Guid id)
        {
            Product product = await _productService.GetById(id);

            FullProductInfoResponse productToUpdate =
                _mapper.Map<FullProductInfoResponse>(product,
                    opt => opt.Items["DbContext"] = _dbContext);

            return Ok(productToUpdate);
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
        public async Task<ActionResult<List<ProductCard>>> GetProductsForAdmin([FromRoute] int page)
        {
            return Ok(_mapper.Map<List<ProductCard>>(
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
        public async Task<ActionResult<List<ProductCard>>> GetSimmularToProduct(
            [FromRoute] Guid productId)
        {
            return Ok(_mapper.Map<List<ProductCard>>(await _productService.GetSimmularToProduct(productId),
                opt => opt.Items["DbContext"] = _dbContext).Take(10));
        }

        [HttpGet("get-most-popular-products/page{page}")]
        public async Task<IActionResult> GetMostPopularProducts([FromRoute] int page)
        {
            return Ok(_mapper.Map<List<ProductCard>>(
                await _productService.GetProductsByPopularity(page),
                opt => opt.Items["DbContext"] = _dbContext));
        }
    }
}
