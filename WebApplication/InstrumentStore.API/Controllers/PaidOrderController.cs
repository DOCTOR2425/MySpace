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
		private readonly ICartService _cartService;
		private readonly IJwtProvider _jwtProvider;

		public PaidOrderController(ICartService cartService,
			IJwtProvider jwtProvider)
		{
			_cartService = cartService;
			_jwtProvider = jwtProvider;
		}

		[Authorize]
		[HttpGet]
		public async Task<ActionResult<List<PaidOrderItem>>> GetUserCart()
		{
			return Ok(await _cartService.GetAllOrders(
				await _jwtProvider.GetUserIdFromToken(
					HttpContext.Request.Cookies[JwtProvider.AccessCookiesName])));
		}
	}
}
