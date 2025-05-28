using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.DataBase.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InstrumentStore.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize(Roles = "admin")]
	public class PromoCodeController : ControllerBase
	{
		private readonly IPromoCodeService _promoCodeService;

		public PromoCodeController(IPromoCodeService promoCodeService)
		{
			_promoCodeService = promoCodeService;
		}

		[HttpGet("get-all-promo-codes")]
		public async Task<ActionResult<List<PromoCode>>> GetPromoCodes()
		{
			return Ok(await _promoCodeService.GetAll());
		}

		[HttpPost("create-promo-code/{name}/{amount}")]
		public async Task<IActionResult> CreatePromoCode(string name, decimal amount)
		{
			return Ok(await _promoCodeService.Create(name, amount));
		}

		[HttpPut("change-promo-code-status/{promoCodeId}/{status:bool}")]
		public async Task<IActionResult> ChangePromoCodeStatus(Guid promoCodeId, bool status)
		{
			return Ok(await _promoCodeService.ChangeStatus(promoCodeId, status));
		}
	}
}
