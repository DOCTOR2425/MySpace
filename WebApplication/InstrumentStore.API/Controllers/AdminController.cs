using AutoMapper;
using Azure.Core;
using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.Contracts.Some;
using InstrumentStore.Domain.Contracts.User;
using InstrumentStore.Domain.DataBase;
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
        private readonly IProductCategoryService _productCategoryService;
        private readonly ICartService _cartService;
        IProductPropertyService _productPropertyService;
        private readonly IMapper _mapper;
        private readonly InstrumentStoreDBContext _dBContext;

        public AdminController(IUsersService usersService,
            IProductService productService,
            IPaidOrderService paidOrderService,
            IDeliveryMethodService deliveryMethodService,
            IPaymentMethodService paymentMethodService,
            IMapper mapper,
            IBrandService brandService,
            ICountryService countryService,
            IProductCategoryService productCategoryService,
            ICartService cartService,
            IProductPropertyService productPropertyService,
            InstrumentStoreDBContext dBContext)
        {
            _usersService = usersService;
            _productService = productService;
            _paidOrderService = paidOrderService;
            _deliveryMethodService = deliveryMethodService;
            _paymentMethodService = paymentMethodService;
            _brandService = brandService;
            _countryService = countryService;
            _productCategoryService = productCategoryService;
            _mapper = mapper;
            _cartService = cartService;
            _productPropertyService = productPropertyService;
            _dBContext = dBContext;
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
            ProductCategory productCategory = new ProductCategory()
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
                ProductCategory = productCategory,
                Description = "string",
                Price = 14,
                Image = "hammer.jpg",
                Quantity = 100
            };
            await _brandService.Create(brand);
            await _countryService.Create(country);
            await _productCategoryService.Create(productCategory);
            await _productService.Create(product);

            Brand brand2 = new Brand()
            {
                BrandId = Guid.NewGuid(),
                Name = "string2"
            };
            ProductCategory productCategory2 = new ProductCategory()
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
                ProductCategory = productCategory2,
                Description = "string2",
                Price = 14,
                Image = "saw.jpg",
                Quantity = 100
            };
            await _brandService.Create(brand2);
            await _countryService.Create(country2);
            await _productCategoryService.Create(productCategory2);
            await _productService.Create(product2);

            Brand brand3 = new Brand()
            {
                BrandId = Guid.NewGuid(),
                Name = "string3"
            };
            ProductCategory productCategory3 = new ProductCategory()
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
                ProductCategory = productCategory3,
                Description = "string3",
                Price = 14,
                Image = "screwdriver.jpg",
                Quantity = 100
            };
            await _brandService.Create(brand3);
            await _countryService.Create(country3);
            await _productCategoryService.Create(productCategory3);
            await _productService.Create(product3);

            Brand brand4 = new Brand()
            {
                BrandId = Guid.NewGuid(),
                Name = "string4"
            };
            ProductCategory productCategory4 = new ProductCategory()
            {
                ProductCategoryId = Guid.NewGuid(),
                Name = "string4"
            };
            Country country4 = new Country()
            {
                CountryId = Guid.NewGuid(),
                Name = "string4"
            };
            Product product4 = new Product()
            {
                ProductId = Guid.NewGuid(),
                Name = "string4",
                Brand = brand4,
                Country = country4,
                ProductCategory = productCategory4,
                Description = "string4",
                Price = 14,
                Image = "screwdriver.jpg",
                Quantity = 100
            };
            await _brandService.Create(brand4);
            await _countryService.Create(country4);
            await _productCategoryService.Create(productCategory4);
            await _productService.Create(product4);


            //Product propperty
            ProductProperty productProperty11 = new ProductProperty()
            {
                ProductPropertyId = Guid.NewGuid(),
                ProductCategory = productCategory,
                Name = "Вес1"
            };
            ProductProperty productProperty12 = new ProductProperty()
            {
                ProductPropertyId = Guid.NewGuid(),
                ProductCategory = productCategory,
                Name = "Длинна1"
            };
            ProductProperty productProperty13 = new ProductProperty()
            {
                ProductPropertyId = Guid.NewGuid(),
                ProductCategory = productCategory,
                Name = "Ширина1"
            };

            ProductProperty productProperty21 = new ProductProperty()
            {
                ProductPropertyId = Guid.NewGuid(),
                ProductCategory = productCategory2,
                Name = "Вес2"
            };
            ProductProperty productProperty22 = new ProductProperty()
            {
                ProductPropertyId = Guid.NewGuid(),
                ProductCategory = productCategory2,
                Name = "Объём2"
            };
            ProductProperty productProperty23 = new ProductProperty()
            {
                ProductPropertyId = Guid.NewGuid(),
                ProductCategory = productCategory2,
                Name = "Ширина2"
            };

            ProductProperty productProperty31 = new ProductProperty()
            {
                ProductPropertyId = Guid.NewGuid(),
                ProductCategory = productCategory3,
                Name = "Вес3"
            };
            ProductProperty productProperty32 = new ProductProperty()
            {
                ProductPropertyId = Guid.NewGuid(),
                ProductCategory = productCategory3,
                Name = "Длинна3"
            };
            ProductProperty productProperty33 = new ProductProperty()
            {
                ProductPropertyId = Guid.NewGuid(),
                ProductCategory = productCategory3,
                Name = "Ширина3"
            };

            ProductProperty productProperty41 = new ProductProperty()
            {
                ProductPropertyId = Guid.NewGuid(),
                ProductCategory = productCategory4,
                Name = "Вес4"
            };
            ProductProperty productProperty42 = new ProductProperty()
            {
                ProductPropertyId = Guid.NewGuid(),
                ProductCategory = productCategory4,
                Name = "Длинна4"
            };
            ProductProperty productProperty43 = new ProductProperty()
            {
                ProductPropertyId = Guid.NewGuid(),
                ProductCategory = productCategory4,
                Name = "Ширина4"
            };

            await _productPropertyService.CreateProperty(productProperty11);
            await _productPropertyService.CreateProperty(productProperty12);
            await _productPropertyService.CreateProperty(productProperty13);
            await _productPropertyService.CreateProperty(productProperty21);
            await _productPropertyService.CreateProperty(productProperty22);
            await _productPropertyService.CreateProperty(productProperty23);
            await _productPropertyService.CreateProperty(productProperty31);
            await _productPropertyService.CreateProperty(productProperty32);
            await _productPropertyService.CreateProperty(productProperty33);
            await _productPropertyService.CreateProperty(productProperty41);
            await _productPropertyService.CreateProperty(productProperty42);
            await _productPropertyService.CreateProperty(productProperty43);

            ProductPropertyValue value11 = new ProductPropertyValue()
            {
                ProductPropertyValueId = Guid.NewGuid(),
                Product = product,
                ProductProperty = productProperty11,
                Value = "VALUE 1 AT PRODUCT 1"
            };
            ProductPropertyValue value12 = new ProductPropertyValue()
            {
                ProductPropertyValueId = Guid.NewGuid(),
                Product = product,
                ProductProperty = productProperty12,
                Value = "VALUE 2 AT PRODUCT 1"
            };
            ProductPropertyValue value13 = new ProductPropertyValue()
            {
                ProductPropertyValueId = Guid.NewGuid(),
                Product = product,
                ProductProperty = productProperty13,
                Value = "VALUE 3 AT PRODUCT 1"
            };

            ProductPropertyValue value21 = new ProductPropertyValue()
            {
                ProductPropertyValueId = Guid.NewGuid(),
                Product = product2,
                ProductProperty = productProperty21,
                Value = "VALUE 1 AT PRODUCT 2"
            };
            ProductPropertyValue value22 = new ProductPropertyValue()
            {
                ProductPropertyValueId = Guid.NewGuid(),
                Product = product2,
                ProductProperty = productProperty22,
                Value = "VALUE 2 AT PRODUCT 2"
            };
            ProductPropertyValue value23 = new ProductPropertyValue()
            {
                ProductPropertyValueId = Guid.NewGuid(),
                Product = product2,
                ProductProperty = productProperty23,
                Value = "VALUE 3 AT PRODUCT 2"
            };

            ProductPropertyValue value31 = new ProductPropertyValue()
            {
                ProductPropertyValueId = Guid.NewGuid(),
                Product = product3,
                ProductProperty = productProperty31,
                Value = "VALUE 1 AT PRODUCT 3"
            };
            ProductPropertyValue value32 = new ProductPropertyValue()
            {
                ProductPropertyValueId = Guid.NewGuid(),
                Product = product3,
                ProductProperty = productProperty32,
                Value = "VALUE 2 AT PRODUCT 3"
            };
            ProductPropertyValue value33 = new ProductPropertyValue()
            {
                ProductPropertyValueId = Guid.NewGuid(),
                Product = product3,
                ProductProperty = productProperty33,
                Value = "VALUE 3 AT PRODUCT 3"
            };

            ProductPropertyValue value41 = new ProductPropertyValue()
            {
                ProductPropertyValueId = Guid.NewGuid(),
                Product = product4,
                ProductProperty = productProperty41,
                Value = "VALUE 1 AT PRODUCT 4"
            };
            ProductPropertyValue value42 = new ProductPropertyValue()
            {
                ProductPropertyValueId = Guid.NewGuid(),
                Product = product4,
                ProductProperty = productProperty42,
                Value = "VALUE 2 AT PRODUCT 4"
            };
            ProductPropertyValue value43 = new ProductPropertyValue()
            {
                ProductPropertyValueId = Guid.NewGuid(),
                Product = product4,
                ProductProperty = productProperty43,
                Value = "VALUE 3 AT PRODUCT 4"
            };

            await _productPropertyService.CreatePropertyValue(value11);
            await _productPropertyService.CreatePropertyValue(value12);
            await _productPropertyService.CreatePropertyValue(value13);
            await _productPropertyService.CreatePropertyValue(value21);
            await _productPropertyService.CreatePropertyValue(value22);
            await _productPropertyService.CreatePropertyValue(value23);
            await _productPropertyService.CreatePropertyValue(value31);
            await _productPropertyService.CreatePropertyValue(value32);
            await _productPropertyService.CreatePropertyValue(value33);
            await _productPropertyService.CreatePropertyValue(value41);
            await _productPropertyService.CreatePropertyValue(value42);
            await _productPropertyService.CreatePropertyValue(value43);


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
                Price = 0
            };

            PaymentMethod paymentMethod1 = new PaymentMethod()
            {
                PaymentMethodId = Guid.NewGuid(),
                Name = "Картой",
            };
            PaymentMethod paymentMethod2 = new PaymentMethod()
            {
                PaymentMethodId = Guid.NewGuid(),
                Name = "Наличными",
            };

            await _deliveryMethodService.Create(deliveryMethod1);
            await _deliveryMethodService.Create(deliveryMethod2);
            await _paymentMethodService.Create(paymentMethod1);
            await _paymentMethodService.Create(paymentMethod2);


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

            await _cartService.OrderCart(userId,
                deliveryMethod1.DeliveryMethodId,
                paymentMethod2.PaymentMethodId);

            await _cartService.AddToCart(userId, product3.ProductId, 3);
            await _cartService.AddToCart(userId, product4.ProductId, 4);




            return Ok();
        }

        [HttpGet("clearDataBase")]
        public async Task<ActionResult> ClearDatabase()
        {
            _dBContext.Brand.RemoveRange(_dBContext.Brand);
            _dBContext.CartItem.RemoveRange(_dBContext.CartItem);
            _dBContext.Country.RemoveRange(_dBContext.Country);
            _dBContext.DeliveryMethod.RemoveRange(_dBContext.DeliveryMethod);
            _dBContext.PaidOrder.RemoveRange(_dBContext.PaidOrder);
            _dBContext.PaidOrderItem.RemoveRange(_dBContext.PaidOrderItem);
            _dBContext.PaymentMethod.RemoveRange(_dBContext.PaymentMethod);
            _dBContext.Product.RemoveRange(_dBContext.Product);
            _dBContext.ProductArchive.RemoveRange(_dBContext.ProductArchive);
            _dBContext.ProductCategory.RemoveRange(_dBContext.ProductCategory);
            _dBContext.ProductProperty.RemoveRange(_dBContext.ProductProperty);
            _dBContext.ProductPropertyValue.RemoveRange(_dBContext.ProductPropertyValue);
            _dBContext.User.RemoveRange(_dBContext.User);
            _dBContext.UserAdresses.RemoveRange(_dBContext.UserAdresses);
            _dBContext.UserRegistrInfos.RemoveRange(_dBContext.UserRegistrInfos);

            _dBContext.SaveChanges();

            return Ok();
        }
    }
}
