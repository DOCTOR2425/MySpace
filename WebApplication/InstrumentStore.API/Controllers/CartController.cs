using AutoMapper;
using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.Contracts.Cart;
using InstrumentStore.Domain.DataBase.Models;
using InstrumentStore.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InstrumentStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private ICartService _cartService;
        private IDeliveryMethodService _deliveryMethodService;
        private IPaymentMethodService _paymentMethodService;
        private IJwtProvider _jwtProvider;
        private IMapper _mapper;

        public CartController(ICartService cartService,
            IJwtProvider jwtProvider,
            IMapper mapper,
            IDeliveryMethodService deliveryMethodService,
            IPaymentMethodService paymentMethodService)
        {
            _cartService = cartService;
            _jwtProvider = jwtProvider;
            _mapper = mapper;
            _deliveryMethodService = deliveryMethodService;
            _paymentMethodService = paymentMethodService;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<List<CartItemResponse>>> GetUserCart()
        {
            string token = HttpContext.Request.Headers["Authorization"]
                .ToString().Substring("Bearer ".Length).Trim();

            List<CartItem> cartItems = await _cartService.GetAllCart(
                _jwtProvider.GetUserIdFromToken(token));

            List<CartItemResponse> result = new List<CartItemResponse>();

            foreach (CartItem cartItem in cartItems)
                result.Add(_mapper.Map<CartItemResponse>(cartItem));

            Console.WriteLine("Cart Items Sent:", result.Count);

            return Ok(result);
        }

        [Authorize]
        [HttpPost("add-to-cart")]
        public async Task<ActionResult> AddToUserCart([FromBody] AddToCartRequest request)
        {
            return Ok(await _cartService.AddToCart(
                _jwtProvider.GetUserIdFromToken(
                    HttpContext.Request.Cookies[JwtProvider.AccessCookiesName]),
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
        [HttpPost("order-cart")]
        public async Task<ActionResult<Guid>> OrderCart([FromBody] OrderCartRequest orderCartRequest)
        {
            return Ok(await _cartService.OrderCart(
                _jwtProvider.GetUserIdFromToken(
                    HttpContext.Request.Cookies[JwtProvider.AccessCookiesName]),
                orderCartRequest.DeliveryMethodId,
                orderCartRequest.PaymentMethodId));
        }

        [Authorize]
        [HttpPost("order-product")]
        public async Task<ActionResult<Guid>> OrderProduct([FromBody] OrderProductRequest orderProductRequest)
        {
            return Ok(await _cartService.OrderProduct(
                _jwtProvider.GetUserIdFromToken(
                    HttpContext.Request.Cookies[JwtProvider.AccessCookiesName]),
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
    }
}
