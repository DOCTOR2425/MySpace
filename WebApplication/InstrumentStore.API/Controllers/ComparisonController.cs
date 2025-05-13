using AutoMapper;
using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.Contracts.Products;
using InstrumentStore.Domain.DataBase;
using InstrumentStore.Domain.DataBase.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InstrumentStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComparisonController : ControllerBase
    {
        private readonly InstrumentStoreDBContext _dbContext;
        private readonly IProductComparisonService _productComparisonService;
        private readonly IUserService _usersService;
        private readonly IProductPropertyService _productPropertyService;
        private readonly IMapper _mapper;

        public ComparisonController(
            InstrumentStoreDBContext dbContext,
            IProductComparisonService productComparisonService,
            IMapper mapper,
            IUserService usersService,
            IProductPropertyService productPropertyService)
        {
            _dbContext = dbContext;
            _productComparisonService = productComparisonService;
            _mapper = mapper;
            _usersService = usersService;
            _productPropertyService = productPropertyService;
        }

        private async Task<Guid> GetUserIdFromToken()
        {
            return (await _usersService.GetUserFromToken(HttpContext.Request.Headers["Authorization"]
                .ToString().Substring("Bearer ".Length).Trim())).UserId;
        }

        [Authorize]
        [HttpPost("add-to-comparison/{productId:guid}")]
        public async Task<IActionResult> AddToComparison(Guid productId)
        {
            return Ok(await _productComparisonService
                .AddToComparison(await GetUserIdFromToken(), productId));
        }

        [Authorize]
        [HttpDelete("delete-from-comparison/{productId:guid}")]
        public async Task<IActionResult> DeleteFromComparison(Guid productId)
        {
            await _productComparisonService
                .DeleteFromComparison(await GetUserIdFromToken(), productId);

            return Ok();
        }

        [Authorize]
        [HttpGet("get-user-comparison")]
        public async Task<IActionResult> GetUserComparison()
        {
            List<Product> products = await _productComparisonService
                .GetUserComparisonProducts(await GetUserIdFromToken());
            List<ProductResponse> response = new List<ProductResponse>();

            foreach (var product in products)
            {
                var productResponseData = _mapper.Map<ProductData>(product, opt => opt.Items["DbContext"] = _dbContext);

                response.Add(new ProductResponse(
                    productResponseData,
                    await _productPropertyService.GetProductProperties(product.ProductId)));
            }

            return Ok(response);
        }

        [Authorize]
        [HttpDelete("clear-comparison-list")]
        public async Task<IActionResult> ClearComparisonList()
        {
            await _productComparisonService.ClearComparisonList(await GetUserIdFromToken());
            return Ok();
        }
    }
}
