using AutoMapper;
using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.Contracts.Cart;
using InstrumentStore.Domain.Contracts.User;
using InstrumentStore.Domain.DataBase;
using InstrumentStore.Domain.DataBase.Models;
using Microsoft.EntityFrameworkCore;

namespace InstrumentStore.Domain.Services
{
    public class FillDataBaseService : IFillDataBaseService
    {
        private readonly IUsersService _usersService;
        private readonly ICityService _cityService;
        private readonly IProductService _productService;
        private readonly IPaidOrderService _paidOrderService;
        private readonly IDeliveryMethodService _deliveryMethodService;
        private readonly IPaymentMethodService _paymentMethodService;
        private readonly IBrandService _brandService;
        private readonly ICountryService _countryService;
        private readonly IProductCategoryService _productCategoryService;
        private readonly ICartService _cartService;
        private readonly IProductPropertyService _productPropertyService;
        private readonly IImageService _productImageService;
        private readonly IMapper _mapper;
        private readonly InstrumentStoreDBContext _dBContext;

        public FillDataBaseService(IUsersService usersService,
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
            InstrumentStoreDBContext dBContext,
            ICityService cityService,
            IImageService productImageService)
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
            _cityService = cityService;
            _productImageService = productImageService;
        }

        public class FillResult
        {
            public Brand Brand { get; set; }
            public Country Country { get; set; }
            public ProductCategory ProductCategory { get; set; }
            public ProductProperty Property1 { get; set; }
            public ProductProperty Property2 { get; set; }
            public ProductProperty Property3 { get; set; }
        }

        public async Task FillAll()
        {
            List<Product> products = await CreateMakita();
            products.AddRange(await CreateBosch());
            products.AddRange(await CreateDewalt());

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
            City city = new City()
            {
                CityId = Guid.NewGuid(),
                Name = "Минск"
            };

            await _cityService.Create(city);
            await _deliveryMethodService.Create(deliveryMethod1);
            await _deliveryMethodService.Create(deliveryMethod2);


            //User
            RegisterUserRequest registerUserRequest = new RegisterUserRequest(
                "Egor",
                "Dudkin",
                "+375 44 111-11-11",
                "stayler425@yandex.com",
                "stayler425@yandex.com"
            );
            Guid userId = await _usersService.Register(registerUserRequest);

            //User cart
            await _cartService.AddToCart(userId, products[0].ProductId, 1);
            await _cartService.AddToCart(userId, products[2].ProductId, 2);

            OrderRequest orderCartRequest = new OrderRequest()
            {
                DeliveryMethodId = deliveryMethod1.DeliveryMethodId,
                PaymentMethod = PaymentMethodService.PaymentMethods[PaymentMethod.Cash],
                UserDelivaryAddress = new UserDeliveryAddress()
                {
                    City = city.Name,
                    Street = "Матусевича",
                    HouseNumber = "10",
                    Entrance = "1",
                    Flat = "10"
                }
            };

            await _cartService.OrderCartForRegistered(userId, orderCartRequest);

            await _cartService.AddToCart(userId, products[1].ProductId, 3);
            await _cartService.AddToCart(userId, products[3].ProductId, 4);

            await CreateCategories();
        }

        private async Task<FillResult> FillSecondaryScrewdriverFields(
            string brandName,
            string countryName)
        {
            var result = new FillResult();

            if (await _dBContext.Brand.FirstOrDefaultAsync(
                b => b.Name.ToLower() == brandName.ToLower()) != null)
            {
                result.Brand = await _dBContext.Brand.FirstOrDefaultAsync(
                    b => b.Name.ToLower() == brandName.ToLower());
            }
            else
            {
                result.Brand = new Brand()
                {
                    BrandId = Guid.NewGuid(),
                    Name = brandName
                };
                await _brandService.Create(result.Brand);
            }

            if (await _dBContext.Country.FirstOrDefaultAsync(
                c => c.Name.ToLower() == countryName.ToLower()) != null)
            {
                result.Country = await _dBContext.Country.FirstOrDefaultAsync(
                    c => c.Name.ToLower() == countryName.ToLower());
            }
            else
            {
                result.Country = new Country()
                {
                    CountryId = Guid.NewGuid(),
                    Name = countryName
                };
                await _countryService.Create(result.Country);
            }

            if (await _dBContext.ProductCategory.FirstOrDefaultAsync(c => c.Name.ToLower() == "шуруповёрты") != null)
            {
                result.ProductCategory = await _dBContext.ProductCategory.FirstOrDefaultAsync(c => c.Name.ToLower() == "шуруповёрты");
            }
            else
            {
                result.ProductCategory = new ProductCategory()
                {
                    ProductCategoryId = Guid.NewGuid(),
                    Name = "Шуруповёрты"
                };
                await _productCategoryService.Create(result.ProductCategory);
            }

            if (await _dBContext.ProductProperty.FirstOrDefaultAsync(p => p.Name.ToLower() == "тип двигателя") != null)
            {
                result.Property1 = await _dBContext.ProductProperty.FirstOrDefaultAsync(p => p.Name.ToLower() == "тип двигателя");
            }
            else
            {
                result.Property1 = new ProductProperty()
                {
                    ProductPropertyId = Guid.NewGuid(),
                    Name = "Тип двигателя",
                    IsRanged = false,
                    ProductCategory = result.ProductCategory
                };
                await _productPropertyService.CreateProperty(result.Property1);
            }

            if (await _dBContext.ProductProperty.FirstOrDefaultAsync(p => p.Name.ToLower() == "max крутящий момент") != null)
            {
                result.Property2 = await _dBContext.ProductProperty.FirstOrDefaultAsync(p => p.Name.ToLower() == "max крутящий момент");
            }
            else
            {
                result.Property2 = new ProductProperty()
                {
                    ProductPropertyId = Guid.NewGuid(),
                    Name = "Max крутящий момент",
                    IsRanged = true,
                    ProductCategory = result.ProductCategory
                };
                await _productPropertyService.CreateProperty(result.Property2);
            }

            if (await _dBContext.ProductProperty.FirstOrDefaultAsync(p => p.Name.ToLower() == "тип аккумулятора") != null)
            {
                result.Property3 = await _dBContext.ProductProperty.FirstOrDefaultAsync(p => p.Name.ToLower() == "тип аккумулятора");
            }
            else
            {
                result.Property3 = new ProductProperty()
                {
                    ProductPropertyId = Guid.NewGuid(),
                    Name = "Тип аккумулятора",
                    IsRanged = false,
                    ProductCategory = result.ProductCategory
                };
                await _productPropertyService.CreateProperty(result.Property3);
            }

            return result;
        }

        public async Task<List<Product>> CreateMakita()
        {
            var result = await FillSecondaryScrewdriverFields(
            "Makita",
            "Япония");

            Brand brand = result.Brand;
            Country country = result.Country;
            ProductCategory productCategory = result.ProductCategory;
            ProductProperty property1 = result.Property1;
            ProductProperty property2 = result.Property2;
            ProductProperty property3 = result.Property3;

            Product product = new Product()
            {
                ProductId = Guid.NewGuid(),
                Name = "Дрель-шуруповерт Makita аккумулятор G-серия 18 В, 13 мм, 42/24 Нм (2x1,5Ач, з/у) DF488D002",
                Brand = brand,
                Country = country,
                ProductCategory = productCategory,
                Description = @"Особенности:
                    Быстрозажимной патрон обеспечивает легкую смену оснастки
                    Надежный редуктор с металлическими шестернями и защитой от пыли
                    Эргономичная рукоятка с резиновыми вставками
                    Вес всего 1,7 кг способствует упрощённой работе без усталости
                    Предусмотрен удобный кейс для хранения и транспортировки
                    Поставляется в комплекте с двумя аккумуляторами(ёмкостью 1, 5 А),
                    кейсом и зарядным устройством.",
                Price = 240,
                Quantity = 100
            };
            await _productService.Create(product);
            Image image = new Image()
            {
                ImageId = Guid.NewGuid(),
                Name = "шуруповёрт2.png",
                Index = 0,
                Product = product,
            };
            await _productImageService.Create(image);
            ProductPropertyValue value11 = new ProductPropertyValue()
            {
                ProductPropertyValueId = Guid.NewGuid(),
                Product = product,
                ProductProperty = property1,
                Value = "безщёточный"
            };
            ProductPropertyValue value12 = new ProductPropertyValue()
            {
                ProductPropertyValueId = Guid.NewGuid(),
                Product = product,
                ProductProperty = property2,
                Value = "42"
            };
            ProductPropertyValue value13 = new ProductPropertyValue()
            {
                ProductPropertyValueId = Guid.NewGuid(),
                Product = product,
                ProductProperty = property3,
                Value = "Li-Ion2"
            };
            await _productPropertyService.CreatePropertyValue(value11);
            await _productPropertyService.CreatePropertyValue(value12);
            await _productPropertyService.CreatePropertyValue(value13);

            Product product2 = new Product()
            {
                ProductId = Guid.NewGuid(),
                Name = "Аккумуляторная дрель-шуруповерт Makita LXT DDF485Z",
                Brand = brand,
                Country = country,
                ProductCategory = productCategory,
                Description = @"Аккумуляторная дрель-шуруповерт Makita DDF485Z 
                    используется при сборке мебели, выполнении ремонтных и отделочных 
                    работ в зданиях и на улице. В зависимости от плотности материала, 
                    при заворачивании и выворачивании шурупов, можно выбрать одну из 
                    21-ой настройки муфты ограничения крутящего момента.",
                Price = 120,
                Quantity = 100
            };
            await _productService.Create(product2);
            Image image2 = new Image()
            {
                ImageId = Guid.NewGuid(),
                Name = "шуруповёрт2.png",
                Index = 0,
                Product = product2,
            };
            await _productImageService.Create(image2);
            ProductPropertyValue value21 = new ProductPropertyValue()
            {
                ProductPropertyValueId = Guid.NewGuid(),
                Product = product2,
                ProductProperty = property1,
                Value = "щёточный"
            };
            ProductPropertyValue value22 = new ProductPropertyValue()
            {
                ProductPropertyValueId = Guid.NewGuid(),
                Product = product2,
                ProductProperty = property2,
                Value = "38"
            };
            ProductPropertyValue value23 = new ProductPropertyValue()
            {
                ProductPropertyValueId = Guid.NewGuid(),
                Product = product2,
                ProductProperty = property3,
                Value = "Li-Ion"
            };
            await _productPropertyService.CreatePropertyValue(value21);
            await _productPropertyService.CreatePropertyValue(value22);
            await _productPropertyService.CreatePropertyValue(value23);

            return new List<Product>()
            {
                product,
                product2
            };
        }

        public async Task<List<Product>> CreateBosch()
        {
            var result = await FillSecondaryScrewdriverFields(
                "Bosch",
                "Германия");

            Brand brand = result.Brand;
            Country country = result.Country;
            ProductCategory productCategory = result.ProductCategory;
            ProductProperty property1 = result.Property1;
            ProductProperty property2 = result.Property2;
            ProductProperty property3 = result.Property3;

            Product product = new Product()
            {
                ProductId = Guid.NewGuid(),
                Name = "Аккумуляторная дрель-шуруповерт Bosch Easydrill 18V-40 06039D8005",
                Brand = brand,
                Country = country,
                ProductCategory = productCategory,
                Description = @"Аккумуляторная дрель-шуруповерт Bosch Easydrill 18V-40 06039D8005 
                    предназначена для закручивания и откручивания винтов, а также для 
                    сверления в древесине, металле, керамике и пластмассе.Универсальный 
                    13 мм патрон с одной втулкой – для легкой замены бит-насадок и сверл.",
                Price = 200,
                Quantity = 100
            };
            await _productService.Create(product);
            Image image = new Image()
            {
                ImageId = Guid.NewGuid(),
                Product = product,
                Index = 0,
                Name = "шуруповёрт.png"
            };
            await _productImageService.Create(image);
            ProductPropertyValue value11 = new ProductPropertyValue()
            {
                ProductPropertyValueId = Guid.NewGuid(),
                Product = product,
                ProductProperty = property1,
                Value = "щёточный"
            };
            ProductPropertyValue value12 = new ProductPropertyValue()
            {
                ProductPropertyValueId = Guid.NewGuid(),
                Product = product,
                ProductProperty = property2,
                Value = "40"
            };
            ProductPropertyValue value13 = new ProductPropertyValue()
            {
                ProductPropertyValueId = Guid.NewGuid(),
                Product = product,
                ProductProperty = property3,
                Value = "Li-Ion1"
            };
            await _productPropertyService.CreatePropertyValue(value11);
            await _productPropertyService.CreatePropertyValue(value12);
            await _productPropertyService.CreatePropertyValue(value13);

            Product product2 = new Product()
            {
                ProductId = Guid.NewGuid(),
                Name = "Аккумуляторная дрель-шуруповерт Bosch AdvancedDrill 18 06039B5009",
                Brand = brand,
                Country = country,
                ProductCategory = productCategory,
                Description = @"Аккумуляторная дрель-шуруповерт Bosch AdvancedDrill 
                    18 06039B5009 с интуитивно понятным управлением помогает 
                    решать различные задачи по обработке древесины, пластмассы и металла.",
                Price = 180,
                Quantity = 100
            };
            await _productService.Create(product2);
            Image image2 = new Image()
            {
                ImageId = Guid.NewGuid(),
                Product = product2,
                Index = 0,
                Name = "шуруповёрт.png"
            };
            await _productImageService.Create(image2);
            ProductPropertyValue value21 = new ProductPropertyValue()
            {
                ProductPropertyValueId = Guid.NewGuid(),
                Product = product2,
                ProductProperty = property1,
                Value = "щёточный"
            };
            ProductPropertyValue value22 = new ProductPropertyValue()
            {
                ProductPropertyValueId = Guid.NewGuid(),
                Product = product2,
                ProductProperty = property2,
                Value = "36"
            };
            ProductPropertyValue value23 = new ProductPropertyValue()
            {
                ProductPropertyValueId = Guid.NewGuid(),
                Product = product2,
                ProductProperty = property3,
                Value = "Li-Ion"
            };
            await _productPropertyService.CreatePropertyValue(value21);
            await _productPropertyService.CreatePropertyValue(value22);
            await _productPropertyService.CreatePropertyValue(value23);

            return new List<Product>()
            {
                product,
                product2
            };
        }

        public async Task<List<Product>> CreateDewalt()
        {
            var result = await FillSecondaryScrewdriverFields(
                "DEWALT",
                "США");

            Brand brand = result.Brand;
            Country country = result.Country;
            ProductCategory productCategory = result.ProductCategory;
            ProductProperty property1 = result.Property1;
            ProductProperty property2 = result.Property2;
            ProductProperty property3 = result.Property3;

            Product product = new Product()
            {
                ProductId = Guid.NewGuid(),
                Name = "Бесщеточная дрель-шуруповерт DEWALT 18.0 В XR DCD777S2T",
                Brand = brand,
                Country = country,
                ProductCategory = productCategory,
                Description = @"Бесщеточная дрель-шуруповерт Dewalt 18.0 В XR DCD777S2T 
                    используется для работ с крепежными элементами и сверления отверстий.
                    Быстрозажимной патрон обеспечивает простую замену насадок, что 
                    позволяет экономить время. В зависимости от плотности материала 
                    можно выбрать один из пятнадцати режимов крутящего момента. 
                    Кейс решает вопрос хранения и транспортировки. ",
                Price = 290,
                Quantity = 100
            };
            await _productService.Create(product);
            Image image = new Image()
            {
                ImageId = Guid.NewGuid(),
                Product = product,
                Index = 0,
                Name = "шуруповёрт3.png"
            };
            await _productImageService.Create(image);
            ProductPropertyValue value11 = new ProductPropertyValue()
            {
                ProductPropertyValueId = Guid.NewGuid(),
                Product = product,
                ProductProperty = property1,
                Value = "безщёточный"
            };
            ProductPropertyValue value12 = new ProductPropertyValue()
            {
                ProductPropertyValueId = Guid.NewGuid(),
                Product = product,
                ProductProperty = property2,
                Value = "65"
            };
            ProductPropertyValue value13 = new ProductPropertyValue()
            {
                ProductPropertyValueId = Guid.NewGuid(),
                Product = product,
                ProductProperty = property3,
                Value = "Li-Ion"
            };
            await _productPropertyService.CreatePropertyValue(value11);
            await _productPropertyService.CreatePropertyValue(value12);
            await _productPropertyService.CreatePropertyValue(value13);

            Product product2 = new Product()
            {
                ProductId = Guid.NewGuid(),
                Name = "Компактная дрель-шуруповерт DEWALT 12 В XR DCD701D2-QW",
                Brand = brand,
                Country = country,
                ProductCategory = productCategory,
                Description = @"Компактная дрель-шуруповерт Dewalt 12 В XR 
                    DCD701D2-QW предназначен для ремонтно-строительных работ.
                    Работа от аккумулятора позволяет использовать инструмент 
                    в местах, где нет возможности подключиться к сети.
                    Предусмотрена встроенная подсветка для работы в 
                    местах с плохим освещением",
                Price = 215,
                Quantity = 100
            };
            await _productService.Create(product2);
            Image image2 = new Image()
            {
                ImageId = Guid.NewGuid(),
                Product = product2,
                Index = 0,
                Name = "шуруповёрт3.png"
            };
            await _productImageService.Create(image2);
            ProductPropertyValue value21 = new ProductPropertyValue()
            {
                ProductPropertyValueId = Guid.NewGuid(),
                Product = product2,
                ProductProperty = property1,
                Value = "безщёточный"
            };
            ProductPropertyValue value22 = new ProductPropertyValue()
            {
                ProductPropertyValueId = Guid.NewGuid(),
                Product = product2,
                ProductProperty = property2,
                Value = "57.5"
            };
            ProductPropertyValue value23 = new ProductPropertyValue()
            {
                ProductPropertyValueId = Guid.NewGuid(),
                Product = product2,
                ProductProperty = property3,
                Value = "Li-Ion2"
            };
            await _productPropertyService.CreatePropertyValue(value21);
            await _productPropertyService.CreatePropertyValue(value22);
            await _productPropertyService.CreatePropertyValue(value23);

            return new List<Product>()
            {
                product,
                product2
            };
        }

        public async Task FillProducts()
        {
            await CreateMakita();
            await CreateBosch();
            await CreateDewalt();
        }

        private async Task CreateCategories()
        {
            ProductCategory productCategory1 = new ProductCategory()
            {
                ProductCategoryId = Guid.NewGuid(),
                Name = "Молотки"
            };
            ProductCategory productCategory2 = new ProductCategory()
            {
                ProductCategoryId = Guid.NewGuid(),
                Name = "Отвёртки"
            };
            ProductCategory productCategory3 = new ProductCategory()
            {
                ProductCategoryId = Guid.NewGuid(),
                Name = "Пилы"
            };
            await _productCategoryService.Create(productCategory1);
            await _productCategoryService.Create(productCategory2);
            await _productCategoryService.Create(productCategory3);

            // 1 Молотки
            ProductProperty productProperty11 = new ProductProperty()
            {
                ProductPropertyId = Guid.NewGuid(),
                Name = "Вес",
                IsRanged = true,
                ProductCategory = productCategory1
            };
            ProductProperty productProperty12 = new ProductProperty()
            {
                ProductPropertyId = Guid.NewGuid(),
                Name = "Материал рукояти",
                IsRanged = false,
                ProductCategory = productCategory1
            };
            ProductProperty productProperty13 = new ProductProperty()
            {
                ProductPropertyId = Guid.NewGuid(),
                Name = "Материал головки",
                IsRanged = false,
                ProductCategory = productCategory1
            };
            await _productPropertyService.CreateProperty(productProperty11);
            await _productPropertyService.CreateProperty(productProperty12);
            await _productPropertyService.CreateProperty(productProperty13);

            //2 Отвёртки
            ProductProperty productProperty21 = new ProductProperty()
            {
                ProductPropertyId = Guid.NewGuid(),
                Name = "Вес",
                IsRanged = true,
                ProductCategory = productCategory2
            };
            ProductProperty productProperty22 = new ProductProperty()
            {
                ProductPropertyId = Guid.NewGuid(),
                Name = "Материал рукояти",
                IsRanged = false,
                ProductCategory = productCategory2
            };
            ProductProperty productProperty23 = new ProductProperty()
            {
                ProductPropertyId = Guid.NewGuid(),
                Name = "Материал головки",
                IsRanged = false,
                ProductCategory = productCategory2
            };
            await _productPropertyService.CreateProperty(productProperty21);
            await _productPropertyService.CreateProperty(productProperty22);
            await _productPropertyService.CreateProperty(productProperty23);

            //3 Пилы
            ProductProperty productProperty31 = new ProductProperty()
            {
                ProductPropertyId = Guid.NewGuid(),
                Name = "Длинна",
                IsRanged = true,
                ProductCategory = productCategory3
            };
            ProductProperty productProperty32 = new ProductProperty()
            {
                ProductPropertyId = Guid.NewGuid(),
                Name = "Угол зубцов",
                IsRanged = true,
                ProductCategory = productCategory3
            };
            ProductProperty productProperty33 = new ProductProperty()
            {
                ProductPropertyId = Guid.NewGuid(),
                Name = "Направление зубцов",
                IsRanged = false,
                ProductCategory = productCategory3
            };
            await _productPropertyService.CreateProperty(productProperty31);
            await _productPropertyService.CreateProperty(productProperty32);
            await _productPropertyService.CreateProperty(productProperty33);
        }

        public async Task ClearDatabase()
        {
            _dBContext.Brand.RemoveRange(_dBContext.Brand);
            _dBContext.CartItem.RemoveRange(_dBContext.CartItem);
            _dBContext.City.RemoveRange(_dBContext.City);
            _dBContext.Comment.RemoveRange(_dBContext.Comment);
            _dBContext.Country.RemoveRange(_dBContext.Country);
            _dBContext.DeliveryAddress.RemoveRange(_dBContext.DeliveryAddress);
            _dBContext.DeliveryMethod.RemoveRange(_dBContext.DeliveryMethod);
            _dBContext.Image.RemoveRange(_dBContext.Image);
            _dBContext.PaidOrder.RemoveRange(_dBContext.PaidOrder);
            _dBContext.PaidOrderItem.RemoveRange(_dBContext.PaidOrderItem);
            _dBContext.Product.RemoveRange(_dBContext.Product);
            _dBContext.ProductCategory.RemoveRange(_dBContext.ProductCategory);
            _dBContext.ProductComparisonItem.RemoveRange(_dBContext.ProductComparisonItem);
            _dBContext.ProductProperty.RemoveRange(_dBContext.ProductProperty);
            _dBContext.ProductPropertyValue.RemoveRange(_dBContext.ProductPropertyValue);
            _dBContext.User.RemoveRange(_dBContext.User);
            _dBContext.UserRegistrInfo.RemoveRange(_dBContext.UserRegistrInfo);

            _dBContext.SaveChanges();
        }
    }
}
