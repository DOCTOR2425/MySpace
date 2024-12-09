using InstrumentStore.Domain.DataBase.Models;
using InstrumentStore.Domain.Abstractions;
using Microsoft.AspNetCore.Mvc;
using InstrumentStore.Domain.Contracts.Products;
using AutoMapper;

namespace InstrumentStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IMapper _mapper;

        public ProductController(IProductService productService, IMapper mapper)
        {
            _productService = productService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProductCard>>> GetAllProductsCards()
        {
            List<Product> products = await _productService.GetAll();
            List<ProductCard> productsCards = new List<ProductCard>();

            foreach (var p in products)
                productsCards.Add(_mapper.Map<ProductCard>(p));

            return Ok(productsCards);
        }

        [HttpGet("/{id}")]
        public async Task<ActionResult<ProductResponse>> GetProduct([FromRoute] Guid id)
        {
            var product = await _productService.GetById(id);
            var response = _mapper.Map<ProductResponse>(product);

            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateProduct([FromBody] ProductRequest productRequest)
        {
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
/*
{
  "name": "CheckRequest",
  "description": "И ещё ForPath",
  "price": 30,
  "quantity": 30,
  "image": "QA==",
  "productTypeId": "dd6995d4-30fe-4637-a34d-1f7d8ef2f53b",
  "brandId": "f0c2afe2-70d3-4a4f-b9c9-490a8020730f",
  "countryId": "7a0e0584-7c19-403f-bf9f-20753e9cd175"
}
*/