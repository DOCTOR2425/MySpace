using AutoMapper;
using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.Contracts.Comment;
using InstrumentStore.Domain.Contracts.PaidOrders;
using InstrumentStore.Domain.Contracts.Products;
using InstrumentStore.Domain.Contracts.User;
using InstrumentStore.Domain.DataBase;
using InstrumentStore.Domain.DataBase.Models;
using InstrumentStore.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace InstrumentStore.API.Controllers
{
	[Authorize]
	[Route("api/[controller]")]
	[ApiController]
	public class UserController : ControllerBase
	{
		private readonly InstrumentStoreDBContext _dbContext;
		private readonly IUsersService _usersService;
		private readonly IAdminService _adminService;
		private readonly IPaidOrderService _paidOrderService;
		private readonly ICommentService _commentService;
		private readonly IProductService _productService;
		private readonly IImageService _imageService;
		private readonly IMapper _mapper;

		public UserController(
			IUsersService usersService,
			IAdminService adminService,
			IMapper mapper,
			InstrumentStoreDBContext dbContext,
			IPaidOrderService paidOrderService,
			ICommentService commentService,
			IProductService productService,
			IImageService imageService)
		{
			_usersService = usersService;
			_adminService = adminService;
			_mapper = mapper;
			_dbContext = dbContext;
			_paidOrderService = paidOrderService;
			_commentService = commentService;
			_productService = productService;
			_imageService = imageService;
		}

		private async Task<User> GetUserFromToken()
		{
			return await _usersService.GetUserFromToken(HttpContext.Request.Headers["Authorization"]
				.ToString().Substring("Bearer ".Length).Trim());
		}

		[AllowAnonymous]
		private async Task<string> ClientLogin(string email, string password)
		{
			string token = "";

			if (await _adminService.IsAdminEmail(email))
				token = await _adminService.Login(email, password);
			else
				token = await _usersService.Login(email, password);

			HttpContext.Response.Cookies.Append(JwtProvider.AccessCookiesName, token, new CookieOptions()
			{// выдача пользователю токена в куки файлы
				Expires = DateTime.Now.Add(JwtProvider.CookiesLifeTime)
			});

			string role = (new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken)
				.Claims.First(c => c.Type == ClaimTypes.Role).Value;

			return role;
		}

		[AllowAnonymous]
		[HttpPost("register")]// функция регистрации пользователя
		public async Task<ActionResult<string>> Register([FromBody] RegisterUserRequest request)
		{
			Guid userId = await _usersService.Register(request);

			string role = await ClientLogin(request.Email, request.Password);

			return Ok(new { role });
		}

		[AllowAnonymous]
		[HttpPost("login")]// функция входа пользователя в аккаунт
		public async Task<ActionResult<string>> Login([FromBody] LoginUserRequest request)
		{
			string role = await ClientLogin(request.Email, request.Password);

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
			return Ok(await GetUserProfileResponse(await GetUserFromToken()));
		}

		[HttpPost("update-user")]
		public async Task<ActionResult<UserProfileResponse>> UpdateUser(
			[FromBody] UpdateUserRequest updateUserRequest)
		{
			User user = await GetUserFromToken();

			await _usersService.Update(user.UserId, updateUserRequest);

			return Ok(await GetUserProfileResponse(user));
		}

		private async Task<UserProfileResponse> GetUserProfileResponse(User user)
		{
			UserProfileResponse response = _mapper.Map<UserProfileResponse>(user);

			response.PendingReviewNumber = (await _usersService.GetOrderedProductsPendingReviewsByUser(user.UserId)).Count;
			response.OrderNumber = (await _paidOrderService.GetAllByUserId(user.UserId)).Count;
			response.CommentNumber = (await _commentService.GetCommentsByUser(user.UserId)).Count;

			DeliveryAddress? address = await _paidOrderService.GetLastAddressByUser(user.UserId);
			if (address == null)
				return response;

			response.City = address.City.Name;
			response.Street = address.Street;
			response.HouseNumber = address.HouseNumber;
			response.Entrance = address.Entrance;
			response.Flat = address.Flat;

			return response;
		}

		[HttpGet("get-paid-orders")]
		public async Task<ActionResult<List<UserPaidOrderResponse>>> GetPaidOrders()
		{
			List<PaidOrder> paidOrders = await _paidOrderService.GetAllByUserId((await GetUserFromToken()).UserId);

			List<UserPaidOrderResponse> orderResponses = _mapper
				.Map<List<UserPaidOrderResponse>>(paidOrders,
					opt => opt.Items["DbContext"] = _dbContext);

			return Ok(orderResponses);
		}

		[HttpGet("get-user-comments")]
		public async Task<IActionResult> GetUserComments()
		{
			List<CommentForUserResponse> comments = _mapper.Map<List<CommentForUserResponse>>(await
				_commentService.GetCommentsByUser((await GetUserFromToken()).UserId));

			foreach (CommentForUserResponse comment in comments)
				comment.Image = "https://localhost:7295/images/" +
					(await _imageService.GetByProductId(comment.ProductId))[0].Name;

			return Ok(comments);
		}

		[HttpGet("get-ordered-products-pending-reviews")]
		public async Task<ActionResult<List<ProductCard>>> GetOrderedProductsPendingReviews()
		{
			return Ok(_mapper.Map<List<ProductCard>>(
				await _usersService.GetOrderedProductsPendingReviewsByUser(
					(await GetUserFromToken()).UserId),
					opt => opt.Items["DbContext"] = _dbContext));
		}
	}
}
