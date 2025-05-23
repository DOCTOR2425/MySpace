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
		private readonly IUserService _usersService;
		private readonly IAccountService _accountService;
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
		private readonly Random _random = new Random();

		public FillDataBaseService(IUserService usersService,
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
			IImageService productImageService,
			IAccountService accountService)
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
			_productImageService = productImageService;
			_accountService = accountService;
		}

		public async Task FillAll()
		{
			await ClearDatabase();
			await CreateCategoriesAndProducts();
			await CreateDeliveryMethods();
			await CreateTestUserAndCart();
		}

		private async Task CreateCategoriesAndProducts()
		{
			var categories = new List<ProductCategory>
			{
				new ProductCategory { Name = "Шуруповёрты" },
				new ProductCategory { Name = "Дрели" },
				new ProductCategory { Name = "Перфораторы" },
				new ProductCategory { Name = "Болгарки" },
				new ProductCategory { Name = "Лобзики" },
				new ProductCategory { Name = "Пилы дисковые" },
				new ProductCategory { Name = "Фрезеры" },
				new ProductCategory { Name = "Шлифмашины" },
				new ProductCategory { Name = "Пылесосы строительные" },
				new ProductCategory { Name = "Фены строительные" }
			};

			foreach (var category in categories)
			{
				category.ProductCategoryId = Guid.NewGuid();
				await _productCategoryService.Create(category);
				await CreatePropertiesForCategory(category);
				await CreateProductsForCategory(category, 10);
			}
		}

		private async Task CreatePropertiesForCategory(ProductCategory category)
		{
			var properties = new List<ProductProperty>();

			switch (category.Name)
			{
				case "Шуруповёрты":
					properties.AddRange(new[]
					{
						new ProductProperty { Name = "Макс. крутящий момент (Нм)", IsRanged = true },
						new ProductProperty { Name = "Тип аккумулятора", IsRanged = false },
						new ProductProperty { Name = "Ёмкость аккумулятора (Ач)", IsRanged = true },
						new ProductProperty { Name = "Скорость вращения (об/мин)", IsRanged = true }
					});
					break;

				case "Дрели":
					properties.AddRange(new[]
					{
						new ProductProperty { Name = "Мощность (Вт)", IsRanged = true },
						new ProductProperty { Name = "Тип патрона", IsRanged = false },
						new ProductProperty { Name = "Макс. диаметр сверления (мм)", IsRanged = true },
						new ProductProperty { Name = "Реверс", IsRanged = false }
					});
					break;

				case "Перфораторы":
					properties.AddRange(new[]
					{
						new ProductProperty { Name = "Мощность (Вт)", IsRanged = true },
						new ProductProperty { Name = "Энергия удара (Дж)", IsRanged = true },
						new ProductProperty { Name = "Режимы работы", IsRanged = false },
						new ProductProperty { Name = "Вес (кг)", IsRanged = true }
					});
					break;

				case "Болгарки":
					properties.AddRange(new[]
					{
						new ProductProperty { Name = "Мощность (Вт)", IsRanged = true },
						new ProductProperty { Name = "Диаметр диска (мм)", IsRanged = true },
						new ProductProperty { Name = "Скорость вращения (об/мин)", IsRanged = true },
						new ProductProperty { Name = "Защита от перегрузки", IsRanged = false }
					});
					break;

				case "Лобзики":
					properties.AddRange(new[]
					{
						new ProductProperty { Name = "Мощность (Вт)", IsRanged = true },
						new ProductProperty { Name = "Глубина пропила (мм)", IsRanged = true },
						new ProductProperty { Name = "Частота ходов (ход/мин)", IsRanged = true },
						new ProductProperty { Name = "Маятниковый ход", IsRanged = false }
					});
					break;

				case "Пилы дисковые":
					properties.AddRange(new[]
					{
						new ProductProperty { Name = "Мощность (Вт)", IsRanged = true },
						new ProductProperty { Name = "Диаметр диска (мм)", IsRanged = true },
						new ProductProperty { Name = "Глубина пропила (мм)", IsRanged = true },
						new ProductProperty { Name = "Наклон диска", IsRanged = false }
					});
					break;

				case "Фрезеры":
					properties.AddRange(new[]
					{
						new ProductProperty { Name = "Мощность (Вт)", IsRanged = true },
						new ProductProperty { Name = "Частота вращения (об/мин)", IsRanged = true },
						new ProductProperty { Name = "Глубина фрезерования (мм)", IsRanged = true },
						new ProductProperty { Name = "Плавный пуск", IsRanged = false }
					});
					break;

				case "Шлифмашины":
					properties.AddRange(new[]
					{
						new ProductProperty { Name = "Мощность (Вт)", IsRanged = true },
						new ProductProperty { Name = "Скорость вращения (об/мин)", IsRanged = true },
						new ProductProperty { Name = "Размер подошвы (мм)", IsRanged = false },
						new ProductProperty { Name = "Регулировка скорости", IsRanged = false }
					});
					break;

				case "Пылесосы строительные":
					properties.AddRange(new[]
					{
						new ProductProperty { Name = "Мощность (Вт)", IsRanged = true },
						new ProductProperty { Name = "Объём бака (л)", IsRanged = true },
						new ProductProperty { Name = "Уровень шума (дБ)", IsRanged = true },
						new ProductProperty { Name = "Автоматическое сматывание шнура", IsRanged = false }
					});
					break;

				case "Фены строительные":
					properties.AddRange(new[]
					{
						new ProductProperty { Name = "Мощность (Вт)", IsRanged = true },
						new ProductProperty { Name = "Температура воздуха (℃)", IsRanged = true },
						new ProductProperty { Name = "Расход воздуха (л/мин)", IsRanged = true },
						new ProductProperty { Name = "Количество насадок", IsRanged = true }
					});
					break;
			}

			foreach (var property in properties)
			{
				property.ProductPropertyId = Guid.NewGuid();
				property.ProductCategory = category;
				await _productPropertyService.CreateProperty(property);
			}
		}

		private async Task CreateProductsForCategory(ProductCategory category, int count)
		{
			var brands = new[] { "Bosch", "Makita", "DeWalt", "Hitachi", "Metabo", "AEG", "Black+Decker", "Einhell", "Stanley", "Ryobi" };
			var countries = new[] { "Германия", "Япония", "США", "Китай", "Швейцария", "Швеция", "Франция", "Италия", "Южная Корея", "Тайвань" };

			for (int i = 1; i <= count; i++)
			{
				var brandName = brands[_random.Next(brands.Length)];
				var countryName = countries[_random.Next(countries.Length)];

				Brand brand = await GetOrCreateBrand(brandName);
				Country country = await GetOrCreateCountry(countryName);

				var product = new Product
				{
					ProductId = Guid.NewGuid(),
					Name = GenerateProductName(category.Name, brandName, i),
					Brand = brand,
					Country = country,
					ProductCategory = category,
					Description = GenerateProductDescription(category.Name, brandName),
					Price = _random.Next(50, 1000),
					Quantity = _random.Next(5, 100)
				};

				await _productService.Create(product);

				// Добавляем изображение
				var image = new Image
				{
					ImageId = Guid.NewGuid(),
					Name = $"{category.Name.ToLower()}{(i % 3) + 1}.png",
					Index = 0,
					Product = product
				};
				await _productImageService.Create(image);

				// Добавляем свойства товара
				var properties = await _dBContext.ProductProperty
					.Where(p => p.ProductCategory.ProductCategoryId == category.ProductCategoryId)
					.ToListAsync();

				foreach (var property in properties)
				{
					var value = GeneratePropertyValue(property, category.Name);
					var propertyValue = new ProductPropertyValue
					{
						ProductPropertyValueId = Guid.NewGuid(),
						Product = product,
						ProductProperty = property,
						Value = value
					};
					await _productPropertyService.CreatePropertyValue(propertyValue);
				}
			}
		}

		private string GenerateProductName(string category, string brand, int index)
		{
			return category switch
			{
				"Шуруповёрты" => $"{brand} Шуруповёрт {index * 10}Нм ({_random.Next(12, 20)}В)",
				"Дрели" => $"{brand} Дрель {_random.Next(500, 1500)}Вт",
				"Перфораторы" => $"{brand} Перфоратор {_random.Next(2, 8)}Дж",
				"Болгарки" => $"{brand} УШМ {_random.Next(115, 230)}мм",
				"Лобзики" => $"{brand} Лобзик {_random.Next(500, 1000)}Вт",
				"Пилы дисковые" => $"{brand} Циркулярная пила {_random.Next(160, 250)}мм",
				"Фрезеры" => $"{brand} Фрезер {_random.Next(800, 2000)}Вт",
				"Шлифмашины" => $"{brand} Шлифмашина {_random.Next(125, 230)}мм",
				"Пылесосы строительные" => $"{brand} Пылесос {_random.Next(1000, 3000)}Вт",
				"Фены строительные" => $"{brand} Термофен {_random.Next(500, 2000)}Вт",
				_ => $"{brand} {category} {index}"
			};
		}

		private string GenerateProductDescription(string category, string brand)
		{
			return category switch
			{
				"Шуруповёрты" => $"Аккумуляторный шуруповёрт {brand} с удобной рукояткой и быстрозажимным патроном. Идеален для монтажных работ и сборки мебели.",
				"Дрели" => $"Мощная дрель {brand} с реверсом и регулировкой скорости. Подходит для сверления в различных материалах.",
				"Перфораторы" => $"Профессиональный перфоратор {brand} с несколькими режимами работы. Отлично справляется с бетоном и кирпичом.",
				"Болгарки" => $"Угловая шлифовальная машина {brand} с защитой от перегрузок. Подходит для резки и шлифовки металла.",
				"Лобзики" => $"Электрический лобзик {brand} с маятниковым ходом. Позволяет делать точные и чистые резы.",
				"Пилы дисковые" => $"Циркулярная пила {brand} с возможностью наклона диска. Обеспечивает ровные и точные резы.",
				"Фрезеры" => $"Фрезер {brand} с регулировкой скорости. Подходит для обработки кромок и пазов.",
				"Шлифмашины" => $"Эксцентриковая шлифмашина {brand} с системой пылеудаления. Обеспечивает качественную шлифовку поверхностей.",
				"Пылесосы строительные" => $"Строительный пылесос {brand} с большим баком для пыли. Эффективно собирает строительный мусор.",
				"Фены строительные" => $"Термофен {brand} с регулировкой температуры. Подходит для удаления краски, сушки и термоусадки.",
				_ => $"Качественный инструмент {brand} для профессионального использования."
			};
		}

		private string GeneratePropertyValue(ProductProperty property, string categoryName)
		{
			if (property.IsRanged)
			{
				return property.Name switch
				{
					"Макс. крутящий момент (Нм)" => _random.Next(10, 150).ToString(),
					"Ёмкость аккумулятора (Ач)" => (_random.Next(15, 50) * 0.1).ToString("0.0"),
					"Скорость вращения (об/мин)" => _random.Next(500, 3000).ToString(),
					"Мощность (Вт)" => _random.Next(300, 2500).ToString(),
					"Энергия удара (Дж)" => _random.Next(1, 10).ToString(),
					"Вес (кг)" => _random.Next(1, 15).ToString(),
					"Диаметр диска (мм)" => _random.Next(115, 230).ToString(),
					"Глубина пропила (мм)" => _random.Next(30, 100).ToString(),
					"Частота ходов (ход/мин)" => _random.Next(1000, 4000).ToString(),
					"Глубина фрезерования (мм)" => _random.Next(30, 80).ToString(),
					"Объём бака (л)" => _random.Next(10, 50).ToString(),
					"Уровень шума (дБ)" => _random.Next(60, 90).ToString(),
					"Температура воздуха (℃)" => _random.Next(300, 650).ToString(),
					"Расход воздуха (л/мин)" => _random.Next(200, 800).ToString(),
					"Количество насадок" => _random.Next(1, 5).ToString(),
					_ => _random.Next(1, 100).ToString()
				};
			}
			else
			{
				return property.Name switch
				{
					"Тип аккумулятора" => _random.Next(0, 2) == 0 ? "Li-Ion" : "Ni-Cd",
					"Тип патрона" => _random.Next(0, 2) == 0 ? "Быстрозажимной" : "Ключевой",
					"Реверс" => _random.Next(0, 2) == 0 ? "Да" : "Нет",
					"Режимы работы" => _random.Next(0, 2) == 0 ? "Сверление/удар" : "Сверление/долбление/удар",
					"Защита от перегрузки" => _random.Next(0, 2) == 0 ? "Да" : "Нет",
					"Маятниковый ход" => _random.Next(0, 2) == 0 ? "Да" : "Нет",
					"Наклон диска" => _random.Next(0, 2) == 0 ? "45 градусов" : "90 градусов",
					"Плавный пуск" => _random.Next(0, 2) == 0 ? "Да" : "Нет",
					"Размер подошвы (мм)" => _random.Next(0, 2) == 0 ? "125x125" : "150x150",
					"Регулировка скорости" => _random.Next(0, 2) == 0 ? "Да" : "Нет",
					"Автоматическое сматывание шнура" => _random.Next(0, 2) == 0 ? "Да" : "Нет",
					_ => _random.Next(0, 2) == 0 ? "Да" : "Нет"
				};
			}
		}

		private async Task<Brand> GetOrCreateBrand(string brandName)
		{
			var brand = await _dBContext.Brand.FirstOrDefaultAsync(b => b.Name == brandName);
			if (brand == null)
			{
				brand = new Brand { BrandId = Guid.NewGuid(), Name = brandName };
				await _brandService.Create(brand.Name);
				brand = await _dBContext.Brand.FirstOrDefaultAsync(b => b.Name == brandName);
			}
			return brand;
		}

		private async Task<Country> GetOrCreateCountry(string countryName)
		{
			var country = await _dBContext.Country.FirstOrDefaultAsync(c => c.Name == countryName);
			if (country == null)
			{
				country = new Country { CountryId = Guid.NewGuid(), Name = countryName };
				await _countryService.Create(country.Name);
				country = await _dBContext.Country.FirstOrDefaultAsync(c => c.Name == countryName);
			}
			return country;
		}

		private async Task CreateDeliveryMethods()
		{
			var deliveryMethods = new[]
			{
				new DeliveryMethod { DeliveryMethodId = Guid.NewGuid(), Name = "Доставка до дома", Price = 6 },
				new DeliveryMethod { DeliveryMethodId = Guid.NewGuid(), Name = "Самовывоз", Price = 0 },
				new DeliveryMethod { DeliveryMethodId = Guid.NewGuid(), Name = "Экспресс-доставка", Price = 12 }
			};

			foreach (var method in deliveryMethods)
			{
				await _deliveryMethodService.Create(method);
			}
		}

		private async Task CreateTestUserAndCart()
		{
			var registerUserRequest = new RegisterUserRequest(
				"Test",
				"User",
				"+375 29 123-45-67",
				"proegora2006@gmail.com"
			);
			Guid userId = await _accountService.CreateUser(registerUserRequest);

			// Добавляем случайные товары в корзину
			var products = await _dBContext.Product.Take(5).ToListAsync();
			foreach (var product in products)
			{
				await _cartService.AddToCart(userId, product.ProductId, _random.Next(1, 5));
			}

			// Создаем тестовый заказ
			var deliveryMethod = await _dBContext.DeliveryMethod.FirstAsync();
			var orderRequest = new OrderRequest
			{
				DeliveryMethodId = deliveryMethod.DeliveryMethodId,
				PaymentMethod = PaymentMethodService.PaymentMethods[PaymentMethod.Cash],
				UserDelivaryAddress = new UserDeliveryAddress
				{
					City = "Минск",
					Street = "Тестовая",
					HouseNumber = "1",
					Entrance = "1",
					Flat = "1"
				}
			};

			await _cartService.OrderCartForRegistered(userId, orderRequest);
		}

		public async Task ClearDatabase()
		{
			_dBContext.Brand.RemoveRange(_dBContext.Brand);
			_dBContext.CartItem.RemoveRange(_dBContext.CartItem);
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

			await _dBContext.SaveChangesAsync();
		}
	}
}