using AutoMapper;
using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.Contracts.PaidOrders;
using InstrumentStore.Domain.Contracts.Products;
using InstrumentStore.Domain.Contracts.Some;
using InstrumentStore.Domain.DataBase;
using InstrumentStore.Domain.DataBase.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace InstrumentStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class AdminController : ControllerBase// отвечает за все действия админа
    {
        private readonly InstrumentStoreDBContext _dbContext;
        private readonly IDeliveryMethodService _deliveryMethodService;
        private readonly IPaymentMethodService _paymentMethodService;
        private readonly IFillDataBaseService _fillDataBaseService;
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
        private readonly IReportService _reportService;

        public AdminController(IUsersService usersService,
            IDeliveryMethodService deliveryMethodService,
            IPaymentMethodService paymentMethodService,
            IMapper mapper,
            InstrumentStoreDBContext dbContext,
            IFillDataBaseService fillDataBaseService,
            IConfiguration config,
            IAdminService adminService,
            IPaidOrderService paidOrderService,
            IImageService imageService,
            IBrandService brandService,
            ICountryService countryService,
            IProductPropertyService productPropertyService,
            IProductCategoryService productCategoryService,
            IReportService reportService)
        {
            _deliveryMethodService = deliveryMethodService;
            _paymentMethodService = paymentMethodService;
            _dbContext = dbContext;
            _fillDataBaseService = fillDataBaseService;
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
            _reportService = reportService;
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

        [HttpGet("generate-word-report")]
        public async Task<ActionResult> generateWordReport(
            [FromQuery] DateTime from,
            [FromQuery] DateTime to)
        {
            string filePath = await _reportService.GenerateWordReport(from, to);

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);

            string mimeType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

            return File(fileBytes, mimeType, Path.GetFileName(filePath));
        }

        [HttpGet("generate-report-sales-by-category-over-time")]
        public async Task<ActionResult> GenerateReportSalesByCategoryOverTime(
            [FromQuery] DateTime from,
            [FromQuery] DateTime to)
        {
            string filePath = await _reportService.GenerateReportSalesByCategoryOverTime(from, to);

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);

            string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            return File(fileBytes, mimeType, Path.GetFileName(filePath));
        }

        [HttpGet("generate-stock-report-over-time")]
        public async Task<ActionResult> GenerateStockReportOverTime(
            [FromQuery] DateTime from,
            [FromQuery] DateTime to)
        {
            string filePath = await _reportService.GenerateStockReportOverTime(from, to);

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);

            string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            return File(fileBytes, mimeType, Path.GetFileName(filePath));
        }
    }
}
