using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.DataBase.Models;
using Microsoft.AspNetCore.Mvc;

namespace InstrumentStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductTypeController : ControllerBase
    {
        private readonly IProductCategoryService _productCategoryService;

        public ProductTypeController(IProductCategoryService productCategoryService)
        {
            _productCategoryService = productCategoryService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProductCategory>>> GetAllProductTypes()
        {
            return await _productCategoryService.GetAll();
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateProductType([FromBody] string productCategoryName)
        {
            ProductCategory productCategory = new ProductCategory
            {
                ProductCategoryId = Guid.NewGuid(),
                Name = productCategoryName
            };

            return Ok(await _productCategoryService.Create(productCategory));
        }

    }
}
