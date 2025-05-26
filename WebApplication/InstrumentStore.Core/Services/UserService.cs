using AutoMapper;
using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.Contracts.User;
using InstrumentStore.Domain.DataBase;
using InstrumentStore.Domain.DataBase.Models;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace InstrumentStore.Domain.Services
{
	public class UserService : IUserService
	{
		private readonly InstrumentStoreDBContext _dbContext;
		private readonly IJwtProvider _jwtProvider;
		private readonly IMapper _mapper;

		public UserService(
			InstrumentStoreDBContext dbContext,
			IJwtProvider jwtProvider,
			IMapper mapper)
		{
			_dbContext = dbContext;
			_jwtProvider = jwtProvider;
			_mapper = mapper;
		}

		public async Task<JwtSecurityToken> GetRefreshToken(JwtSecurityToken accessToken)
		{
			User user = await GetById(await _jwtProvider.GetUserIdFromToken(accessToken));

			return new JwtSecurityTokenHandler().ReadToken(
				user.RefreshToken) as JwtSecurityToken;
		}

		public async Task<User?> GetByEmail(string email)
		{
			User? user = await _dbContext.User
				.FirstOrDefaultAsync(
				u => u.Email == email);

			return user;
		}

		public async Task<User> GetById(Guid id)
		{
			User? user = await _dbContext.User
				.FirstOrDefaultAsync(u => u.UserId == id);

			if (user == null)
				throw new UnauthorizedAccessException("No user with that Id");

			return user;
		}

		public async Task<User> GetUserFromToken(string token)
		{
			Guid userId = await _jwtProvider.GetUserIdFromToken(token);
			User user = await GetById(userId);

			return user;
		}

		public async Task<User> Update(Guid userId, UpdateUserRequest newUser)
		{
			User? withSameEmail = await GetByEmail(newUser.Email);
			if (withSameEmail != null && withSameEmail.UserId != userId)
				throw new ArgumentException("Exist user with same email");

			User user = await GetById(userId);
			user.FirstName = newUser.FirstName;
			user.Surname = newUser.Surname;
			user.Telephone = newUser.Telephone;
			user.Email = newUser.Email;

			await _dbContext.SaveChangesAsync();
			return user;
		}

		public async Task<DeliveryAddress?> GetLastUserDeliveryAddress(Guid userId)
		{
			User user = await GetById(userId);

			List<PaidOrder> paidOrders = await _dbContext.PaidOrder
				.Include(o => o.DeliveryMethod)
				.Where(o => o.User.UserId == user.UserId)
				.OrderByDescending(o => o.OrderDate)
				.ToListAsync();

			DeliveryAddress? address = null;
			foreach (var order in paidOrders)
			{
				address = _dbContext.DeliveryAddress
					.FirstOrDefault(a => a.PaidOrder.PaidOrderId == order.PaidOrderId);

				if (address != null)
					break;
			}

			return address;
		}

		public async Task<List<Product>> GetOrderedProductsPendingReviewsByUser(Guid userId)
		{
			List<Product> products = new List<Product>();

			List<Product> productsBuff = new List<Product>();
			List<PaidOrder> paidOrders = await _dbContext.PaidOrder
				.AsQueryable()
				.Where(o => o.User.UserId == userId)
				.ToListAsync();

			if (paidOrders.Any() == false)
				return products;

			foreach (var order in paidOrders)
			{
				productsBuff.AddRange(await _dbContext.PaidOrderItem
					.AsQueryable()
					.Include(i => i.Product)
					.Where(i => i.PaidOrder.PaidOrderId == order.PaidOrderId)
					.Select(i => i.Product)
					.ToListAsync());
			}

			foreach (var product in productsBuff)
			{
				if (await _dbContext.Comment
					.AsQueryable()
					.Where(c => c.Product.ProductId == product.ProductId)
					.AnyAsync() == false)
				{
					products.Add(product);
				}
			}

			return products;
		}

		public async Task<List<User>> GetUsersForAdmin(
			string? searchQuery,
			DateTime? dateFrom,
			DateTime? dateTo,
			bool? isBlocked,
			bool? hasOrders)
		{
			List<User> users = await _dbContext.User
				.Where(u =>
					(!dateFrom.HasValue || u.RegistrationDate >= dateFrom) &&
					(!dateTo.HasValue || u.RegistrationDate <= dateTo))
				.ToListAsync();

			if (isBlocked.HasValue)
				users = FilterBlockedUsers(users, isBlocked.Value);

			if (hasOrders.HasValue)
				users = await FilterUsersByOrsed(users, hasOrders.Value);

			if (searchQuery != null)
				users = FilterUsersByQuery(users, searchQuery);

			return users;
		}

		private List<User> FilterUsersByQuery(List<User> users, string query)
		{
			query = query.ToLower();
			var filteredUsers = new List<User>();

			foreach (var user in users)
			{
				if ((user.FirstName + user.Surname).ToLower().Contains(query) ||
						user.Email.ToLower().Contains(query) ||
						user.Telephone.ToLower().Contains(query))
					filteredUsers.Add(user);
			}

			return filteredUsers;
		}

		private async Task<List<User>> FilterUsersByOrsed(List<User> users, bool hasOrders)
		{
			var filteredUsers = new List<User>();

			foreach (var user in users)
			{
				bool orders = await _dbContext.PaidOrder
					.Where(c => c.User.UserId == user.UserId)
					.CountAsync() > 0;
				if (orders == hasOrders)
					filteredUsers.Add(user);
			}

			return filteredUsers;
		}

		private List<User> FilterBlockedUsers(List<User> users, bool isBlocked)
		{
			var filteredUsers = new List<User>();

			foreach (var user in users)
			{
				if (user.BlockDate != null == isBlocked)
					filteredUsers.Add(user);
			}

			return filteredUsers;
		}

		public async Task<AdminUserResponse> GetAdminUserResponse(User user)
		{
			AdminUserResponse adminUserResponse = _mapper.Map<AdminUserResponse>(user);
			adminUserResponse.OrderCount = await _dbContext.PaidOrder
				.Where(o => o.User.UserId == user.UserId)
				.CountAsync();

			return adminUserResponse;
		}

		public async Task<Guid> BlockUser(Guid userId, string details)
		{
			User user = await GetById(userId);

			user.BlockDate = DateTime.Now;
			user.BlockDetails = details;

			await _dbContext.SaveChangesAsync();

			return userId;
		}

		public async Task<Guid> UnblockUser(Guid userId)
		{
			User user = await GetById(userId);

			user.BlockDate = null;
			user.BlockDetails = null;

			await _dbContext.SaveChangesAsync();

			return userId;
		}
	}
}
