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
		private readonly IDeliveryMethodService _deliveryMethodService;
		private readonly IPaymentMethodService _paymentMethodService;
		private readonly IFillDataBaseService _fillDataBaseService;
		private readonly IPaidOrderService _paidOrderService;
		private readonly IProductService _productService;
		private readonly IImageService _imageService;
		private readonly IAdminService _adminService;
		private readonly IMapper _mapper;
		private readonly IConfiguration _config;
		private readonly InstrumentStoreDBContext _dbContext;

		public AdminController(IUsersService usersService,
			IDeliveryMethodService deliveryMethodService,
			IPaymentMethodService paymentMethodService,
			IMapper mapper,
			InstrumentStoreDBContext dbContext,
			IFillDataBaseService fillDataBaseService,
			IConfiguration config,
			IAdminService adminService,
			IPaidOrderService paidOrderService,
			IImageService imageService)
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
			List<AdminPaidOrderResponse> paidOrderResponses = new List<AdminPaidOrderResponse>();
			List<PaidOrder> paidOrders = await _paidOrderService.GetProcessingOrders();

			foreach (PaidOrder order in paidOrders)
				paidOrderResponses.Add(_mapper.Map<AdminPaidOrderResponse>(order,
					opt => opt.Items["DbContext"] = _dbContext));

			return Ok(paidOrderResponses);
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

	}
}
