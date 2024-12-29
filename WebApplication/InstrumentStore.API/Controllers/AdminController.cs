using AutoMapper;
using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.Contracts.Some;
using InstrumentStore.Domain.DataBase.Models;
using Microsoft.AspNetCore.Mvc;

namespace InstrumentStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IUsersService _usersService;
        private readonly IProductService _productService;
        private readonly IPaidOrderService _paidOrderService;
        private readonly IDeliveryMethodService _deliveryMethodService;
        private readonly IPaymentMethodService _paymentMethodService;
        private readonly IBrandService _brandService;
        private readonly ICountryService _countryService;
        private readonly IProductTypeService _productTypeService;
        private readonly IMapper _mapper;

        public AdminController(IUsersService usersService,
            IProductService productService,
            IPaidOrderService paidOrderService,
            IDeliveryMethodService deliveryMethodService,
            IPaymentMethodService paymentMethodService,
            IMapper mapper,
            IBrandService brandService,
            ICountryService countryService,
            IProductTypeService productTypeService)
        {
            _usersService = usersService;
            _productService = productService;
            _paidOrderService = paidOrderService;
            _deliveryMethodService = deliveryMethodService;
            _paymentMethodService = paymentMethodService;
            _brandService = brandService;
            _countryService = countryService;
            _productTypeService = productTypeService;
            _mapper = mapper;
        }

        [HttpPost("create-delivery-method")]
        public async Task<ActionResult<Guid>> CreateDeliveryMethod(
            [FromBody] CreateDeliveryMethodRequest request)
        {
            DeliveryMethod deliveryMethod = new DeliveryMethod()
            {
                DeliveryMethodId = Guid.NewGuid(),
                Name = request.Name,
                Price = request.Price
            };

            return Ok(await _deliveryMethodService.Create(deliveryMethod));
        }

        [HttpPost("create-payment-method")]
        public async Task<ActionResult<Guid>> CreatePaymentMethod([FromBody] string paymentMethodName)
        {
            PaymentMethod paymentMethod = new PaymentMethod()
            {
                PaymentMethodId = Guid.NewGuid(),
                Name = paymentMethodName
            };

            return Ok(await _paymentMethodService.Create(paymentMethod));
        }

        [HttpGet("FillOneProduct")]
        public async Task<Guid> FillOneProduct()
        {
            Brand brand = new Brand()
            {
                BrandId = Guid.NewGuid(),
                Name = "string"
            };

            ProductType productType = new ProductType()
            {
                ProductTypeId = Guid.NewGuid(),
                Name = "string"
            };

            Country country = new Country()
            {
                CountryId = Guid.NewGuid(),
                Name = "string"
            };

            Product product = new Product()
            {
                ProductId = Guid.NewGuid(),
                Name = "string",
                Brand = brand,
                Country = country,
                ProductType = productType,
                Description = "string",
                Price = 14,
                Image = "hammer.jpg",
                Quantity = 100
            };

            await _brandService.Create(brand);
            await _countryService.Create(country);
            await _productTypeService.Create(productType);
            await _productService.Create(product);

            return product.ProductId;
        }
    }
}
