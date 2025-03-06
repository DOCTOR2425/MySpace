using AutoMapper;
using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.Contracts.Cart;
using InstrumentStore.Domain.Contracts.User;
using InstrumentStore.Domain.DataBase;
using InstrumentStore.Domain.DataBase.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InstrumentStore.API.Controllers
{
	[Authorize]
	[Route("api/[controller]")]
	[ApiController]
	public class CartController : ControllerBase
	{
		private readonly ICartService _cartService;
		private readonly IDeliveryMethodService _deliveryMethodService;
		private readonly IPaymentMethodService _paymentMethodService;
		private readonly IUsersService _usersService;
		private readonly IJwtProvider _jwtProvider;
		private readonly IMapper _mapper;
		private readonly InstrumentStoreDBContext _dbContext;

		public CartController(ICartService cartService,
			IJwtProvider jwtProvider,
			IMapper mapper,
			IDeliveryMethodService deliveryMethodService,
			IPaymentMethodService paymentMethodService,
			IUsersService usersService,
			InstrumentStoreDBContext dbContext)
		{
			_cartService = cartService;
			_deliveryMethodService = deliveryMethodService;
			_paymentMethodService = paymentMethodService;
			_usersService = usersService;
			_jwtProvider = jwtProvider;
			_mapper = mapper;
			_dbContext = dbContext;
		}

		private string GetToken()
		{
			return HttpContext.Request.Headers["Authorization"]
				.ToString().Substring("Bearer ".Length).Trim();
		}

		[HttpGet]
		public async Task<ActionResult<List<CartItemResponse>>> GetUserCart()
		{
			List<CartItem> cartItems = await _cartService.GetAllCart(
				await _jwtProvider.GetUserIdFromToken(GetToken()));

			List<CartItemResponse> result = new List<CartItemResponse>();

			foreach (CartItem cartItem in cartItems)
				result.Add(_mapper.Map<CartItemResponse>(cartItem
					, opt => opt.Items["DbContext"] = _dbContext));

			return Ok(result);
		}

		[HttpPost("add-to-cart")]
		public async Task<ActionResult> AddToUserCart([FromBody] AddToCartRequest request)
		{
			return Ok(await _cartService.AddToCart(
				await _jwtProvider.GetUserIdFromToken(GetToken()),
				request.ProductId,
				request.Quantity));
		}

		[HttpPost("change-cart-item-quantity")]
		public async Task<ActionResult> ChangeCartItemQuantity([FromBody] CartItemResponse request)
		{
			return Ok(await _cartService.AddToCart(
				await _jwtProvider.GetUserIdFromToken(GetToken()),
				request.Product.ProductId,
				request.Quantity));
		}

		[HttpDelete("{cartItemId:guid}")]
		public async Task<ActionResult> RemoveFromCart(Guid cartItemId)
		{
			return Ok(await _cartService.RemoveFromCart(cartItemId));
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
		public async Task<ActionResult> OrderCartForUnregistered([FromBody] OrderCartOfUnregisteredRequest request)
		{
			Guid userId = await _usersService.RegisterUserFromOrder(request.User);
			Guid orderId = await _cartService.OrderCartForUnregistered(userId, request);

			return Ok(orderId);
		}

		[HttpPost("order-product")]
		public async Task<ActionResult<Guid>> OrderProduct([FromBody] OrderProductRequest orderProductRequest)
		{
			return Ok(await _cartService.OrderProduct(
				await _jwtProvider.GetUserIdFromToken(GetToken()),
				orderProductRequest.ProductId,
				orderProductRequest.Quantity,
				orderProductRequest.OrderCartRequest));
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
			DeliveryAddress? address = await _usersService.GetLastUserDeliveryAdress(user.UserId);
			if (address == null)
				userOrderInfo.UserDeliveryAddress = new UserDeliveryAddress();
			else
				userOrderInfo.UserDeliveryAddress = _mapper.Map<UserDeliveryAddress>(address);

			return Ok(userOrderInfo);
		}
	}
}
