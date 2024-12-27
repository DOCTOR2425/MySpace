using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.Contracts.User;
using InstrumentStore.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace InstrumentStore.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UserController : ControllerBase
	{
		private readonly IUsersService _usersService;

		public UserController(IUsersService usersService)
		{
			_usersService = usersService;
		}

		[HttpPost("/register")]
		public async Task<ActionResult<Guid>> Register([FromBody] RegisterUserRequest request)
		{
			return Ok(await _usersService.Register(request));
		}

		[HttpPost("/login")]
		public async Task<ActionResult> Login([FromBody] LoginUserRequest request)
		{
			string token = await _usersService.Login(request.EMail, request.Password);

			HttpContext.Response.Cookies.Append(JwtProvider.AccessCookiesName, token, new CookieOptions()
			{
				Secure = true,
				SameSite = SameSiteMode.Lax,
				Expires = DateTime.Now.AddMinutes(JwtProvider.RefreshTokenLifeDays)
			});

			return Ok();
		}
	}
}
