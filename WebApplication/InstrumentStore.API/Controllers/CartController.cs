using AutoMapper;
using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.Contracts.Cart;
using InstrumentStore.Domain.Contracts.User;
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
        private readonly IDeliveryMethodService _deliveryMethodService;
        private readonly IPaymentMethodService _paymentMethodService;
        private readonly IUsersService _usersService;
        private readonly IJwtProvider _jwtProvider;
        private readonly IMapper _mapper;

        public CartController(ICartService cartService,
            IJwtProvider jwtProvider,
            IMapper mapper,
            IDeliveryMethodService deliveryMethodService,
            IPaymentMethodService paymentMethodService,
            IUsersService usersService)
        {
            _cartService = cartService;
            _deliveryMethodService = deliveryMethodService;
            _paymentMethodService = paymentMethodService;
            _usersService = usersService;
            _jwtProvider = jwtProvider;
            _mapper = mapper;
        }

        private string GetToken()
        {
            return HttpContext.Request.Headers["Authorization"]
                .ToString().Substring("Bearer ".Length).Trim();
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<List<CartItemResponse>>> GetUserCart()
        {
            List<CartItem> cartItems = await _cartService.GetAllCart(
                _jwtProvider.GetUserIdFromToken(GetToken()));

            List<CartItemResponse> result = new List<CartItemResponse>();

            foreach (CartItem cartItem in cartItems)
                result.Add(_mapper.Map<CartItemResponse>(cartItem));

            return Ok(result);
        }

        [Authorize]
        [HttpPost("add-to-cart")]
        public async Task<ActionResult> AddToUserCart([FromBody] AddToCartRequest request)
        {
            return Ok(await _cartService.AddToCart(
                _jwtProvider.GetUserIdFromToken(GetToken()),
                request.ProductId,
                request.Quantity));
        }

        [Authorize]
        [HttpDelete("{cartItemId:guid}")]
        public async Task<ActionResult> RemoveFromCart(Guid cartItemId)
        {
            return Ok(await _cartService.RemoveFromCart(cartItemId));
        }

        [Authorize]
        [HttpPost("order-cart-for-registered")]
        public async Task<ActionResult<Guid>> OrderCart([FromBody] OrderCartRequest orderCartRequest)
        {
            return Ok(await _cartService.OrderCart(
                _jwtProvider.GetUserIdFromToken(GetToken()),
                orderCartRequest.DeliveryMethodId,
                orderCartRequest.PaymentMethodId));
        }

        [Authorize]
        [HttpPost("order-product")]
        public async Task<ActionResult<Guid>> OrderProduct([FromBody] OrderProductRequest orderProductRequest)
        {
            return Ok(await _cartService.OrderProduct(
                _jwtProvider.GetUserIdFromToken(GetToken()),
                orderProductRequest.ProductId,
                orderProductRequest.Quantity,
                orderProductRequest.DeliveryMethodId,
                orderProductRequest.PaymentMethodId));
        }

        [Authorize]
        [HttpGet("get-order-options")]
        public async Task<ActionResult<OrderOptionsResponse>> GetOrderOptions()
        {
            return Ok(new OrderOptionsResponse(
                await _deliveryMethodService.GetAll(),
                await _paymentMethodService.GetAll()));
        }

        [Authorize]
        [HttpGet("get-user-order-info")]
        public async Task<ActionResult<UserOrderInfo>> GetUserOrderInfo()
        {
            User user = await _usersService.GetById(
                _jwtProvider.GetUserIdFromToken(GetToken()));

            return Ok(_mapper.Map<UserOrderInfo>(user));
        }
    }
}
