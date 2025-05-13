using AutoMapper;
using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.Contracts.PaidOrders;
using InstrumentStore.Domain.Contracts.Products;
using InstrumentStore.Domain.Contracts.Some;
using InstrumentStore.Domain.DataBase;
using InstrumentStore.Domain.DataBase.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;


namespace InstrumentStore.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize(Roles = "admin")]
	public class AdminController : ControllerBase
	{
		private readonly InstrumentStoreDBContext _dbContext;
		private readonly IDeliveryMethodService _deliveryMethodService;
		private readonly IPaymentMethodService _paymentMethodService;
		private readonly IPaidOrderService _paidOrderService;
		private readonly IProductService _productService;
		private readonly IProductPropertyService _productPropertyService;
		private readonly IProductCategoryService _productCategoryService;
		private readonly IBrandService _brandService;
		private readonly ICountryService _countryService;
		private readonly IImageService _imageService;
		private readonly IAdminService _adminService;
		private readonly IMapper _mapper;
		private readonly IConfiguration _config;

		public AdminController(IUserService usersService,
			IDeliveryMethodService deliveryMethodService,
			IPaymentMethodService paymentMethodService,
			IMapper mapper,
			InstrumentStoreDBContext dbContext,
			IConfiguration config,
			IAdminService adminService,
			IPaidOrderService paidOrderService,
			IImageService imageService,
			IBrandService brandService,
			ICountryService countryService,
			IProductPropertyService productPropertyService,
			IProductCategoryService productCategoryService)
		{
			_deliveryMethodService = deliveryMethodService;
			_paymentMethodService = paymentMethodService;
			_dbContext = dbContext;
			_config = config;
			_adminService = adminService;
			_mapper = mapper;
			_dbContext = dbContext;
			_paidOrderService = paidOrderService;
			_imageService = imageService;
			_brandService = brandService;
			_countryService = countryService;
			_productPropertyService = productPropertyService;
			_productCategoryService = productCategoryService;
		}

		[HttpPost("create-delivery-method")]//добавление способа доставки товара
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

		[HttpGet("get-processing-orders")]
		public async Task<ActionResult<List<AdminPaidOrderResponse>>> GetProcessingOrders()
		{
			List<PaidOrder> paidOrders = await _paidOrderService.GetProcessingOrders();

			return Ok(_mapper.Map<List<AdminPaidOrderResponse>>(paidOrders,
					opt => opt.Items["DbContext"] = _dbContext));
		}

		[HttpPut("close-order{orderId:guid}")]
		public async Task<ActionResult> CloseOrder([FromRoute] Guid orderId)
		{
			return Ok(await _paidOrderService.CloseOrder(orderId));
		}

		[HttpPut("cancel-order{orderId:guid}")]
		public async Task<ActionResult> CancelOrder([FromRoute] Guid orderId)
		{
			return Ok(await _paidOrderService.CancelOrder(orderId));
		}

		[HttpGet("get-options-for-product")]
		public async Task<ActionResult<OptionsForProduct>> GetOptionsForProduct()
		{
			List<Brand> brands = await _brandService.GetAll();
			List<Country> countries = await _countryService.GetAll();
			List<ProductCategory> productCategories = await _productCategoryService.GetAll();

			OptionsForProduct options = new OptionsForProduct(brands, countries, productCategories);

			return Ok(options);
		}

		[HttpPost("create-brand/{brandName}")]
		public async Task<IActionResult> CreateBrand([FromRoute] string brandName)
		{
			return Ok(await _brandService.Create(brandName));
		}

		[HttpPost("create-country/{countryName}")]
		public async Task<ActionResult<Guid>> CreateCountry([FromRoute] string countryName)
		{
			return Ok(await _countryService.Create(countryName));
		}

		[HttpGet("get-all-brands")]
		public async Task<ActionResult<List<Brand>>> GetAllBrands()
		{
			return Ok(await _brandService.GetAll());
		}

		[HttpGet("get-all-countries")]
		public async Task<ActionResult<List<Brand>>> GetAllCountries()
		{
			return Ok(await _countryService.GetAll());
		}

		[HttpPut("update-brand/{brandId:guid}/{newName}")]
		public async Task<IActionResult> UpdateBrand(
			[FromRoute] Guid brandId,
			[FromRoute] string newName)
		{
			await _brandService.Update(brandId, newName);
			return Ok(brandId);
		}

		[HttpPut("update-country/{countryId:guid}/{newName}")]
		public async Task<IActionResult> UpdateCountry(
			[FromRoute] Guid countryId,
			[FromRoute] string newName)
		{
			await _countryService.Update(countryId, newName);
			return Ok(countryId);
		}

		[HttpGet("upload-image/{name}")]
		public IActionResult GetImage(string name)
		{
			var decodedName = Uri.UnescapeDataString(name);
			var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", decodedName);

			var provider = new FileExtensionContentTypeProvider();
			if (!provider.TryGetContentType(decodedName, out var contentType))
				contentType = "application/octet-stream";

			return PhysicalFile(path, contentType);
		}
	}
}
