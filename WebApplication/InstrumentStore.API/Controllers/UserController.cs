using AutoMapper;
using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.Contracts.Comment;
using InstrumentStore.Domain.Contracts.PaidOrders;
using InstrumentStore.Domain.Contracts.Products;
using InstrumentStore.Domain.Contracts.User;
using InstrumentStore.Domain.DataBase;
using InstrumentStore.Domain.DataBase.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InstrumentStore.API.Controllers
{
	[Authorize]
	[Route("api/[controller]")]
	[ApiController]
	public class UserController : ControllerBase
	{
		private readonly InstrumentStoreDBContext _dbContext;
		private readonly IUserService _usersService;
		private readonly IPaidOrderService _paidOrderService;
		private readonly ICommentService _commentService;
		private readonly IImageService _imageService;
		private readonly ICartService _cartService;
		private readonly IProductComparisonService _productComparisonService;
		private readonly IProductService _productService;
		private readonly IMapper _mapper;

		public UserController(
			IUserService usersService,
			IMapper mapper,
			InstrumentStoreDBContext dbContext,
			IPaidOrderService paidOrderService,
			ICommentService commentService,
			IImageService imageService,
			ICartService cartService,
			IProductComparisonService productComparisonService,
			IProductService productService)
		{
			_usersService = usersService;
			_mapper = mapper;
			_dbContext = dbContext;
			_paidOrderService = paidOrderService;
			_commentService = commentService;
			_imageService = imageService;
			_cartService = cartService;
			_productComparisonService = productComparisonService;
			_productService = productService;
		}

		private async Task<User> GetUserFromToken()
		{
			return await _usersService.GetUserFromToken(HttpContext.Request.Headers["Authorization"]
				.ToString().Substring("Bearer ".Length).Trim());
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

			response.City = address.City;
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
		public async Task<IActionResult> GetUserComments(
			[FromQuery] Guid? userId = null)
		{
			if (userId == null)
				userId = (await GetUserFromToken()).UserId;

			List<UserCommentResponse> comments = _mapper
				.Map<List<UserCommentResponse>>(
					await _commentService.GetCommentsByUser((Guid)userId));

			foreach (UserCommentResponse comment in comments)
				comment.Image = "https://localhost:7295/images/" +
					(await _imageService.GetByProductId(comment.ProductId))[0].Name;

			return Ok(comments);
		}

		[HttpGet("get-ordered-products-pending-reviews")]
		public async Task<ActionResult<List<UserProductCard>>> GetOrderedProductsPendingReviews()
		{
			Guid userId = (await GetUserFromToken()).UserId;
			return Ok(await _productService.GetUserProductCards(
					await _usersService.GetOrderedProductsPendingReviewsByUser(userId),
				userId));
		}

		[HttpGet("get-user-product-stats/{productId:guid}")]
		public async Task<ActionResult<UserProductStats>> GetUserProductStats([FromRoute] Guid productId)
		{
			Guid userId = (await GetUserFromToken()).UserId;
			UserProductStats userProductStats = new UserProductStats(
				await _cartService.GetProductQuantityInUserCart(productId, userId),
				await _productComparisonService.IsProductInUserComparison(productId, userId));

			return Ok(userProductStats);
		}

		[Authorize(Roles = "admin")]
		[HttpGet("get-users-for-admin")]
		public async Task<ActionResult<List<AdminUserResponse>>> GetUsersForAdmin(
			[FromQuery] int page,
			[FromQuery] string? searchQuery = "",
			[FromQuery] DateTime? dateFrom = null,
			[FromQuery] DateTime? dateTo = null,
			[FromQuery] bool? isBlocked = null,
			[FromQuery] bool? hasOrders = null)
		{
			List<AdminUserResponse> adminUsers = new List<AdminUserResponse>();
			List<User> users = (await _usersService.GetUsersForAdmin(
				searchQuery,
				dateFrom,
				dateTo,
				isBlocked,
				hasOrders))
				.Skip(4 * (page - 1))
				.Take(4)
				.ToList();

			foreach (User user in users)
				adminUsers.Add(await _usersService.GetAdminUserResponse(user));

			return Ok(adminUsers);
		}

		[Authorize(Roles = "admin")]
		[HttpGet("get-user-for-admin/{userId:guid}")]
		public async Task<ActionResult<AdminUserResponse>> GetUserForAdmin(
			[FromRoute] Guid userId)
		{
			User user = await _usersService.GetById(userId);
			return Ok(_mapper.Map<AdminUserResponse>(user));
		}

		[Authorize(Roles = "admin")]
		[HttpPut("block-user/{userId:guid}")]
		public async Task<IActionResult> BlockUser(
			[FromRoute] Guid userId,
			[FromQuery] string blockDetails)
		{
			return Ok(await _usersService.BlockUser(userId, blockDetails));
		}

		[Authorize(Roles = "admin")]
		[HttpPut("unblock-user/{userId:guid}")]
		public async Task<IActionResult> UnblockUser([FromRoute] Guid userId)
		{
			return Ok(await _usersService.UnblockUser(userId));
		}
	}
}
