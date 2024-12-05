using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.DataBase.Models;
using Microsoft.AspNetCore.Mvc;

namespace InstrumentStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Product>>> GetAllProducts()
        {
            var response = await _productService.GetAll();

            return Ok(response);
        }

        [HttpGet("/{id}")]
        public async Task<ActionResult<List<Product>>> GetProduct([FromRoute] Guid id)
        {
            var response = await _productService.GetAll();

            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateProduct([FromBody] Product product)
        {
            return await _productService.Create(product);
        }
    }
}
