using AutoMapper;
using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.Contracts.PaidOrders;
using InstrumentStore.Domain.Contracts.User;
using InstrumentStore.Domain.DataBase;
using InstrumentStore.Domain.DataBase.Models;
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
		private readonly InstrumentStoreDBContext _dbContext;
		private readonly IUsersService _usersService;
		private readonly IAdminService _adminService;
		private readonly IPaidOrderService _paidOrderService;
		private readonly IMapper _mapper;

		public UserController(
			IUsersService usersService,
			IAdminService adminService,
			IMapper mapper,
			InstrumentStoreDBContext dbContext,
			IPaidOrderService paidOrderService)
		{
			_usersService = usersService;
			_adminService = adminService;
			_mapper = mapper;
			_dbContext = dbContext;
			_paidOrderService = paidOrderService;
		}

		private string GetToken()
		{
			return HttpContext.Request.Headers["Authorization"]
				.ToString().Substring("Bearer ".Length).Trim();
		}

		[HttpPost("register")]// функция регистрации пользователя
		public async Task<ActionResult<Guid>> Register([FromBody] RegisterUserRequest request)
		{
			return Ok(await _usersService.Register(request));
		}

		[HttpPost("login")]// функция входа пользователя в аккаунт
		public async Task<ActionResult<string>> Login([FromBody] LoginUserRequest request)
		{
			string token = "";

			try
			{
				if (await _adminService.IsAdminEmail(request.Email))
					token = await _adminService.Login(request.Email, request.Password);
				else
					token = await _usersService.Login(request.Email, request.Password);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return Unauthorized();
			}

			HttpContext.Response.Cookies.Append(JwtProvider.AccessCookiesName, token, new CookieOptions()
			{// выдача пользователю токена в куки файлы
				Expires = DateTime.Now.Add(JwtProvider.CookiesLifeTime)
			});

			string role = (new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken)
				.Claims.First(c => c.Type == ClaimTypes.Role).Value;

			return Ok(new { role });
		}

		[HttpGet("logout")]
		public async Task<ActionResult> Logout()
		{
			HttpContext.Response.Cookies.Delete(JwtProvider.AccessCookiesName);

			return Ok();
		}

		[HttpGet("get-user")]
		public async Task<ActionResult<UserProfileResponse>> GetUser()
		{
			User user = await _usersService.GetUserFromToken(GetToken());

			UserProfileResponse response = _mapper.Map<UserProfileResponse>(user,
				opt => opt.Items["DbContext"] = _dbContext);

			return Ok(response);
		}

		[HttpPost("update-user")]
		public async Task<ActionResult<UserProfileResponse>> UpdateUser(
			[FromBody] UpdateUserRequest updateUserRequest)
		{
			User user = await _usersService.GetUserFromToken(GetToken());

			await _usersService.Update(user.UserId, updateUserRequest);

			UserProfileResponse response = _mapper.Map<UserProfileResponse>(user,
				opt => opt.Items["DbContext"] = _dbContext);

			return Ok(response);
		}

		[HttpGet("get-paid-orders")]
		public async Task<ActionResult<List<PaidOrderResponse>>> GetPaidOrders()
		{
			User user = await _usersService.GetUserFromToken(GetToken());

			List<PaidOrder> paidOrders = await _paidOrderService.GetAll(user.UserId);

			List<PaidOrderResponse> orderResponses = new List<PaidOrderResponse>();

			foreach (PaidOrder order in paidOrders)
			{
				orderResponses.Add(_mapper.Map<PaidOrderResponse>(order,
					opt => opt.Items["DbContext"] = _dbContext));

				Console.WriteLine(orderResponses.Last().PaidOrderItems.Last().Price);
			}

			return Ok(orderResponses);
		}
	}
}
