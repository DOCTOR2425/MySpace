using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.Contracts.User;
using InstrumentStore.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace InstrumentStore.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UserController : ControllerBase
	{
		private readonly IUsersService _usersService;
		private readonly IAdminService _adminService;

		public UserController(
			IUsersService usersService,
			IAdminService adminService)
		{
			_usersService = usersService;
			_adminService = adminService;
		}

		[HttpPost("/register")]
		public async Task<ActionResult<Guid>> Register([FromBody] RegisterUserRequest request)
		{
			return Ok(await _usersService.Register(request));
		}

		[HttpPost("/login")]
		public async Task<ActionResult<string>> Login([FromBody] LoginUserRequest request)
		{
			string token = "";

			try
			{
				if (await _adminService.IsAdminEmail(request.EMail))
					token = await _adminService.Login(request.EMail, request.Password);
				else
					token = await _usersService.Login(request.EMail, request.Password);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return Unauthorized();
			}

			HttpContext.Response.Cookies.Append(JwtProvider.AccessCookiesName, token, new CookieOptions()
			{
				Secure = true,
				SameSite = SameSiteMode.Lax,
				Expires = DateTime.Now.Add(JwtProvider.CookiesLifeTime)
			});

			string role = (new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken)
				.Claims.First(c => c.Type == ClaimTypes.Role).Value;

			return Ok(new { role });
		}
	}
}
