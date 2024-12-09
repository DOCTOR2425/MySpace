using InstrumentStore.Domain.DataBase.Models;
using InstrumentStore.Domain.Abstractions;
using Microsoft.AspNetCore.Mvc;
using InstrumentStore.Domain.Contracts.Products;
using InstrumentStore.Domain.Mapper;
using AutoMapper;

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
        private readonly IMapper _mapper;

        public ProductController(IProductService productService, IBrandService brandService,
            ICountryService countryService, IProductTypeService productTypeService, IMapper mapper)
        {
            _productService = productService;
            _brandService = brandService;
            _countryService = countryService;
            _productTypeService = productTypeService;
            _mapper = mapper;
        }

        private async Task<Product> BuildProductFromReuest(ProductRequest productRequest)
        {  // потому что нужны ссылки на объекты, а не их копии
            //ProductType productType = await _productTypeService.GetById((await _productTypeService.GetAll())
            //    .Where(t => t.Name == productRequest.ProductType).First().ProductTypeId);

            ////ProductType productType = (await _productTypeService.GetAll()).FirstOrDefault(t => t.Name == productRequest.ProductType);//Неробить

            //Brand brand = await _brandService.GetById((await _brandService.GetAll())
            //    .Where(b => b.Name == productRequest.Brand).First().BrandId);

            //Country country = await _countryService.GetById((await _countryService.GetAll())
            //    .Where(c => c.Name == productRequest.Country).First().CountryId);

            //Product product = new Product
            //{
            //    ProductId = Guid.NewGuid(),
            //    Name = productRequest.Name,
            //    Description = productRequest.Description,
            //    Price = productRequest.Price,
            //    Quantity = productRequest.Quantity,
            //    Image = productRequest.Image,

            //    ProductType = productType,
            //    Brand = brand,
            //    Country = country
            //};

            //return product;
            return _mapper.Map<Product>(productRequest);
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
            Product product = await BuildProductFromReuest(productRequest);

            return Ok(await _productService.Create(product));
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<Guid>> UpdateProduct(Guid id, [FromBody] ProductRequest productRequest)
        {
            Product product = await BuildProductFromReuest(productRequest);

            return Ok(await _productService.Update(id, product));
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<Guid>> DeleteProduct(Guid id)
        {
            return Ok(await _productService.Delete(id));
        }
    }
}
