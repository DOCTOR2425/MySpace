using AutoMapper;
using Azure.Core;
using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.Contracts.Some;
using InstrumentStore.Domain.Contracts.User;
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
        private readonly ICartService _cartService;
        private readonly IMapper _mapper;

        public AdminController(IUsersService usersService,
            IProductService productService,
            IPaidOrderService paidOrderService,
            IDeliveryMethodService deliveryMethodService,
            IPaymentMethodService paymentMethodService,
            IMapper mapper,
            IBrandService brandService,
            ICountryService countryService,
            IProductTypeService productTypeService,
            ICartService cartService)
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
            _cartService = cartService;
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

        [HttpGet("FillAll")]
        public async Task<ActionResult> FillOneProduct()
        {
            //Product
            Brand brand = new Brand()
            {
                BrandId = Guid.NewGuid(),
                Name = "string"
            };
            ProductCategory productType = new ProductCategory()
            {
                ProductCategoryId = Guid.NewGuid(),
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
                ProductCategory = productType,
                Description = "string",
                Price = 14,
                Image = "hammer.jpg",
                Quantity = 100
            };
            await _brandService.Create(brand);
            await _countryService.Create(country);
            await _productTypeService.Create(productType);
            await _productService.Create(product);

            Brand brand2 = new Brand()
            {
                BrandId = Guid.NewGuid(),
                Name = "string2"
            };
            ProductCategory productType2 = new ProductCategory()
            {
                ProductCategoryId = Guid.NewGuid(),
                Name = "string2"
            };
            Country country2 = new Country()
            {
                CountryId = Guid.NewGuid(),
                Name = "string2"
            };
            Product product2 = new Product()
            {
                ProductId = Guid.NewGuid(),
                Name = "string2",
                Brand = brand2,
                Country = country2,
                ProductCategory = productType2,
                Description = "string2",
                Price = 14,
                Image = "saw.jpg",
                Quantity = 100
            };
            await _brandService.Create(brand2);
            await _countryService.Create(country2);
            await _productTypeService.Create(productType2);
            await _productService.Create(product2);

            Brand brand3 = new Brand()
            {
                BrandId = Guid.NewGuid(),
                Name = "string3"
            };
            ProductCategory productType3 = new ProductCategory()
            {
                ProductCategoryId = Guid.NewGuid(),
                Name = "string3"
            };
            Country country3 = new Country()
            {
                CountryId = Guid.NewGuid(),
                Name = "string3"
            };
            Product product3 = new Product()
            {
                ProductId = Guid.NewGuid(),
                Name = "string3",
                Brand = brand3,
                Country = country3,
                ProductCategory = productType3,
                Description = "string3",
                Price = 14,
                Image = "screwdriver.jpg",
                Quantity = 100
            };
            await _brandService.Create(brand3);
            await _countryService.Create(country3);
            await _productTypeService.Create(productType3);
            await _productService.Create(product3);
            Brand brand4 = new Brand()
            {
                BrandId = Guid.NewGuid(),
                Name = "string4"
            };
            ProductCategory productType4 = new ProductCategory()
            {
                ProductCategoryId = Guid.NewGuid(),
                Name = "string4"
            };
            Country country4 = new Country()
            {
                CountryId = Guid.NewGuid(),
                Name = "string4"
            };
            Product produc4t = new Product()
            {
                ProductId = Guid.NewGuid(),
                Name = "string4",
                Brand = brand4,
                Country = country4,
                ProductCategory = productType4,
                Description = "string4",
                Price = 14,
                Image = "screwdriver.jpg",
                Quantity = 100
            };
            await _brandService.Create(brand);
            await _countryService.Create(country);
            await _productTypeService.Create(productType);
            await _productService.Create(product);


            // Options
            DeliveryMethod deliveryMethod1 = new DeliveryMethod()
            {
                DeliveryMethodId = Guid.NewGuid(),
                Name = "Доставка до дома",
                Price = 6
            };
            DeliveryMethod deliveryMethod2 = new DeliveryMethod()
            {
                DeliveryMethodId = Guid.NewGuid(),
                Name = "Самовывоз",
                Price = 6
            };

            await _deliveryMethodService.Create(deliveryMethod1);


            // User
            RegisterUserRequest registerUserRequest = new RegisterUserRequest(
                "Egor",
                "Dudkin",
                "Searheevich",
                "+375445555555",
                "aboba",
                "aboba",
                "Minsk",
                "Matusevicha",
                "100",
                "10",
                "10"
            );
            Guid userId = await _usersService.Register(registerUserRequest);

            //User cart
            await _cartService.AddToCart(userId, product.ProductId, 1);
            await _cartService.AddToCart(userId, product2.ProductId, 2);
            //await _cartService.OrderCart(userId, )


            return Ok();
        }
    }
}
