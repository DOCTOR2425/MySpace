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
        private IJwtProvider _jwtProvider;

        public CartController(ICartService cartService, 
            IJwtProvider jwtProvider)
        {
            _cartService = cartService;
            _jwtProvider = jwtProvider;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<List<CartItem>>> GetUserCart()
        {
            return Ok(await _cartService.GetAllCart(
                await _jwtProvider.getUserIdFromToken(
                    HttpContext.Request.Cookies[JwtProvider.AccessCookiesName])));
        }

        [Authorize]
        [HttpPost("add-to-cart")]
        public async Task<ActionResult> AddToUserCart([FromBody] AddToCartRequest request)
        {
            return Ok(await _cartService.AddToCart(
                await _jwtProvider.getUserIdFromToken(
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
                await _jwtProvider.getUserIdFromToken(
                    HttpContext.Request.Cookies[JwtProvider.AccessCookiesName]),
                orderCartRequest.DeliveryMethodId,
                orderCartRequest.PaymentMethodId));
        }

        [Authorize]
        [HttpPost("order-product")]
        public async Task<ActionResult<Guid>> OrderProduct([FromBody] OrderProductRequest orderProductRequest)
        {
            return Ok(await _cartService.OrderProduct(
                await _jwtProvider.getUserIdFromToken(
                    HttpContext.Request.Cookies[JwtProvider.AccessCookiesName]),
                orderProductRequest.ProductId,
                orderProductRequest.Quantity,
                orderProductRequest.DeliveryMethodId,
                orderProductRequest.PaymentMethodId));
        }
    }
}
