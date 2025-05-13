using AutoMapper;
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
	public class AccountController : ControllerBase
	{
		private readonly IUserService _usersService;
		private readonly IAdminService _adminService;
		private readonly IMapper _mapper;
		private readonly IAccountService _accountService;
		private readonly IJwtProvider _jwtProvider;

		public AccountController(
			IUserService usersService,
			IMapper mapper,
			IAccountService accountService,
			IJwtProvider jwtProvider,
			IAdminService adminService)
		{
			_usersService = usersService;
			_mapper = mapper;
			_accountService = accountService;
			_jwtProvider = jwtProvider;
			_adminService = adminService;
		}

		[HttpPost("login-first-stage/{email}")]
		public async Task<IActionResult> LoginFirstStage([FromRoute] string email)
		{
			await ClientLoginFirstStage(email);
			return Ok();
		}

		[HttpPost("register-first-stage")]
		public async Task<IActionResult> RegisterFirstStage([FromBody] RegisterUserRequest request)
		{
			await _accountService.CreateUser(request);
			await ClientLoginFirstStage(request.Email);
			return Ok();
		}

		private async Task ClientLoginFirstStage(string email)
		{
			string code = await _accountService.LoginFirstStage(email);
			string codeHash = await _accountService.GenerateCodeHas(email + code);

			HttpContext.Response.Cookies.Append(AccountService.LoginCode, codeHash, new CookieOptions()
			{
				Expires = DateTime.Now.Add(JwtProvider.CookiesLifeTime)
			});
		}

		[HttpPost("verify-login-code/{email}/{code}")]
		public async Task<IActionResult> VerifyLoginCode(
			[FromRoute] string email,
			[FromRoute] string code)
		{
			string codeHash = HttpContext.Request.Cookies[AccountService.LoginCode];

			string token = "";

			if (await _adminService.IsAdminEmail(email))
				token = await _accountService.AdminLoginSecondStage(email, code, codeHash);
			else
				token = await _accountService.LoginSecondStage(email, code, codeHash);

			HttpContext.Response.Cookies.Append(JwtProvider.AccessCookiesName, token, new CookieOptions()
			{// выдача пользователю токена в куки файлы
				Expires = DateTime.Now.Add(JwtProvider.CookiesLifeTime)
			});

			HttpContext.Response.Cookies.Delete(AccountService.LoginCode);

			string role = (new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken)
				.Claims.First(c => c.Type == ClaimTypes.Role).Value;

			return Ok(new { role });
		}
	}
}
