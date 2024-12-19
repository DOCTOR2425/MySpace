using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.DataBase.Models;
using InstrumentStore.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InstrumentStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaidOrderController : ControllerBase
    {
        private ICartService _cartService;

        public PaidOrderController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<List<CartItem>>> GetUserCart()
        {
            return Ok(await _cartService.GetAllOrders(
                await JwtProvider.getUserIdFromToken(
                    HttpContext.Request.Cookies[JwtProvider.CookiesName])));
        }
    }
}
