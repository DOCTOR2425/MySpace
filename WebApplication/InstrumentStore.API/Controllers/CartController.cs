using AutoMapper;
using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.Contracts.Cart;
using InstrumentStore.Domain.Contracts.Products;
using InstrumentStore.Domain.Contracts.User;
using InstrumentStore.Domain.DataBase;
using InstrumentStore.Domain.DataBase.Models;
using InstrumentStore.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InstrumentStore.API.Controllers
{
	[Authorize]
	[Route("api/[controller]")]
	[ApiController]
	public class CartController : ControllerBase
	{
		private readonly ICartService _cartService;
		private readonly IAccountService _accountService;
		private readonly IDeliveryMethodService _deliveryMethodService;
		private readonly IPaymentMethodService _paymentMethodService;
		private readonly IUserService _usersService;
		private readonly IJwtProvider _jwtProvider;
		private readonly IMapper _mapper;
		private readonly InstrumentStoreDBContext _dbContext;

		public CartController(ICartService cartService,
			IJwtProvider jwtProvider,
			IMapper mapper,
			IDeliveryMethodService deliveryMethodService,
			IPaymentMethodService paymentMethodService,
			IUserService usersService,
			InstrumentStoreDBContext dbContext,
			IAccountService accountService)
		{
			_cartService = cartService;
			_deliveryMethodService = deliveryMethodService;
			_paymentMethodService = paymentMethodService;
			_usersService = usersService;
			_jwtProvider = jwtProvider;
			_mapper = mapper;
			_dbContext = dbContext;
			_accountService = accountService;
		}

		private string GetToken()
		{
			return HttpContext.Request.Headers["Authorization"]
				.ToString().Substring("Bearer ".Length).Trim();
		}

		[HttpGet]
		public async Task<ActionResult<List<CartItemResponse>>> GetUserCart()
		{
			List<CartItem> cartItems = await _cartService.GetUserCartItems(
				await _jwtProvider.GetUserIdFromToken(GetToken()));

			List<CartItemResponse> result = new List<CartItemResponse>();
			foreach (var cartItem in cartItems)
				result.Add(await _cartService.GetCartItemResponse(cartItem));

			return Ok(result);
		}

		[AllowAnonymous]
		[HttpGet("get-product-for-unregestered-cart")]
		public async Task<ActionResult<List<ProductMinimalData>>> GetProductForUnregestereCart(
			[FromQuery] List<Guid> productsId)
		{
			return Ok(await _cartService.GetProductForUnregestereCart(productsId));
		}

		[HttpPost("add-to-cart")]
		public async Task<ActionResult> AddToUserCart([FromBody] AddToCartRequest request)
		{
			return Ok(await _cartService.AddToCart(
				await _jwtProvider.GetUserIdFromToken(GetToken()),
				request.ProductId,
				request.Quantity));
		}

		[HttpDelete("{productId:guid}")]
		public async Task<ActionResult> RemoveFromCart([FromRoute] Guid productId)
		{
			return Ok(await _cartService.RemoveFromCart(productId,
				await _jwtProvider.GetUserIdFromToken(GetToken())));
		}

		[HttpPost("order-cart-for-registered")]
		public async Task<ActionResult<Guid>> OrderCartForRegistered([FromBody] OrderRequest orderCartRequest)
		{
			return Ok(await _cartService.OrderCartForRegistered(
				await _jwtProvider.GetUserIdFromToken(GetToken()),
				orderCartRequest));
		}

		[AllowAnonymous]
		[HttpPost("order-cart-for-unregistered")]
		public async Task<IActionResult> OrderCartForUnregistered([FromBody] OrderCartOfUnregisteredRequest request)
		{
			Guid userId = await _accountService.RegisterUserFromOrder(request.User);
			Guid orderId = await _cartService.OrderCartForUnregistered(userId, request);

			string token = await _jwtProvider.GenerateAccessToken(userId);
			HttpContext.Response.Cookies.Append(JwtProvider.AccessCookiesName, token, new CookieOptions()
			{
				Expires = DateTime.Now.Add(JwtProvider.CookiesLifeTime)
			});

			return Ok(new { orderId });
		}

		[AllowAnonymous]
		[HttpGet("get-order-options")]
		public async Task<ActionResult<OrderOptionsResponse>> GetOrderOptions()
		{
			return Ok(new OrderOptionsResponse(
				await _deliveryMethodService.GetAll(),
				await _paymentMethodService.GetAllToList()));
		}

		[HttpGet("get-user-order-info")]
		public async Task<ActionResult<UserOrderInfo>> GetUserOrderInfo()
		{
			User user = await _usersService.GetById(
				await _jwtProvider.GetUserIdFromToken(GetToken()));

			UserOrderInfo userOrderInfo = _mapper.Map<UserOrderInfo>(user, opt => opt.Items["DbContext"] = _dbContext);
			DeliveryAddress? address = await _usersService.GetLastUserDeliveryAddress(user.UserId);
			if (address == null)
				userOrderInfo.UserDeliveryAddress = new UserDeliveryAddress();
			else
				userOrderInfo.UserDeliveryAddress = _mapper.Map<UserDeliveryAddress>(address);

			return Ok(userOrderInfo);
		}
	}
}
