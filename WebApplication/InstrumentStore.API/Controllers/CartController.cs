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

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<List<CartItem>>> GetUserCart()
        {
            return Ok(await _cartService.GetAllCart(
                await JwtProvider.getUserIdFromToken(
                    HttpContext.Request.Cookies[JwtProvider.CookiesName])));
        }

        [Authorize]
        [HttpPost("add-to-cart")]
        public async Task<ActionResult> AddToUserCart([FromBody] AddToCartRequest request)
        {
            return Ok(await _cartService.AddToCart(
                await JwtProvider.getUserIdFromToken(
                    HttpContext.Request.Cookies[JwtProvider.CookiesName]),
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
                await JwtProvider.getUserIdFromToken(
                    HttpContext.Request.Cookies[JwtProvider.CookiesName]),
                orderCartRequest.DeliveryMethodId,
                orderCartRequest.PaymentMethodId));
        }

        [Authorize]
        [HttpPost("order-product")]
        public async Task<ActionResult<Guid>> OrderProduct([FromBody] OrderProductRequest orderProductRequest)
        {
            return Ok(await _cartService.OrderProduct(
                await JwtProvider.getUserIdFromToken(
                    HttpContext.Request.Cookies[JwtProvider.CookiesName]),
                orderProductRequest.ProductId,
                orderProductRequest.Quantity,
                orderProductRequest.DeliveryMethodId,
                orderProductRequest.PaymentMethodId));
        }
    }
}
