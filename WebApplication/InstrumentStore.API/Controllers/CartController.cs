using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.Contracts.Cart;
using InstrumentStore.Domain.DataBase.Models;
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

        [HttpGet("{userId:guid}")]
        public async Task<ActionResult<List<CartItem>>> GetUserCart(Guid userId)
        {
            return Ok(await _cartService.GetAll(userId));
        }

        [HttpPost("add-to-cart")]
        public async Task<ActionResult> AddToUserCart([FromBody] AddToCartRequest request)
        {
            return Ok(await _cartService.AddToCart(request));
        }

        [HttpDelete("{cartItemId:guid}")]
        public async Task<ActionResult> RemoveFromCart(Guid cartItemId)
        {
            return Ok(await _cartService.RemoveFromCart(cartItemId));
        }

        [HttpPost("order-cart")]
        public async Task<ActionResult<Guid>> OrderCart([FromBody] OrderCartRequest orderCartRequest)
        {
            return Ok(await _cartService.OrderCart(
                orderCartRequest.UserId,
                orderCartRequest.DeliveryMethodId,
                orderCartRequest.PaymentMethodId));
        }

        [HttpPost("order-product")]
        public async Task<ActionResult<Guid>> OrderProduct([FromBody] OrderProductRequest orderProductRequest)
        {
            return Ok(await _cartService.OrderProduct(
                orderProductRequest.UserId,
                orderProductRequest.ProductId,
                orderProductRequest.DeliveryMethodId,
                orderProductRequest.PaymentMethodId));
        }
    }
}
