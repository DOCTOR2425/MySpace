using InstrumentStore.Domain.DataBase.Models;
using InstrumentStore.Domain.Abstractions;
using Microsoft.AspNetCore.Mvc;
using InstrumentStore.Domain.Contracts.Products;

namespace InstrumentStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IBrandService _brandService;
        private readonly IProductTypeService _productTypeService;
        private readonly ICountryService _countryService;

        public ProductController(IProductService productService, IBrandService brandService,
            ICountryService countryService, IProductTypeService productTypeService)
        {
            _productService = productService;
            _brandService = brandService;
            _countryService = countryService;
            _productTypeService = productTypeService;
        }

        private async Task<Product> BuldProductFromResponse(ProductResponse productResponse)
        {
            ProductType productResponseType = await _productTypeService.GetById((await _productTypeService.GetAll())
                .Where(t => t.Name == productResponse.ProductType).First().ProductTypeId);

            Brand brand = await _brandService.GetById((await _brandService.GetAll())
                .Where(b => b.Name == productResponse.Brand).First().BrandId);

            Country country = await _countryService.GetById((await _countryService.GetAll())
                .Where(c => c.Name == productResponse.Country).First().CountryId);

            Product product = new Product
            {
                ProductId = Guid.NewGuid(),
                Name = productResponse.Name,
                Description = productResponse.Description,
                Price = productResponse.Price,
                Quantity = productResponse.Quantity,
                Image = productResponse.Image,

                ProductType = productResponseType,
                Brand = brand,
                Country = country
            };

            return product;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProductCard>>> GetAllProductsCards()
        {
            List<Product> products = await _productService.GetAll();

            List<ProductCard> productsCards = new List<ProductCard>();

            foreach (var p in products)
            {
                productsCards.Add(new ProductCard(
                    p.Name,
                    p.Price,
                    p.Quantity,
                    p.Image,
                    p.ProductType.Name,
                    p.Brand.Name,
                    p.Country.Name));
            }

            return Ok(productsCards);
        }

        [HttpGet("/{id}")]
        public async Task<ActionResult<List<ProductPage>>> GetProduct([FromRoute] Guid id)
        {
            var product = await _productService.GetById(id);

            var response = new ProductPage(
                product.Name,
                product.Price,
                product.Description,
                product.Quantity,
                product.Image,
                product.ProductType.Name,
                product.Brand.Name,
                product.Country.Name);

            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateProduct([FromBody] ProductResponse productResponse)
        {
            Product product = await BuldProductFromResponse(productResponse);

            return Ok(await _productService.Create(product));
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<Guid>> UpdateProduct(Guid id, [FromBody] ProductResponse productResponse)
        {
            Product product = await BuldProductFromResponse(productResponse);

            return Ok(await _productService.Update(id, product));
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<Guid>> DeleteProduct(Guid id)
        {
            return Ok(await _productService.Delete(id));
        }
    }
}
