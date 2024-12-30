using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.DataBase.Models;
using Microsoft.AspNetCore.Mvc;

namespace InstrumentStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductTypeController : ControllerBase
    {
        private readonly IProductTypeService _productTypeService;

        public ProductTypeController(IProductTypeService productTypeService)
        {
            _productTypeService = productTypeService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProductCategory>>> GetAllProductTypes()
        {
            return await _productTypeService.GetAll();
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateProductType([FromBody] string productTypeName)
        {
            ProductCategory productType = new ProductCategory
            {
                ProductCategoryId = Guid.NewGuid(),
                Name = productTypeName
            };

            return Ok(await _productTypeService.Create(productType));
        }

    }
}
