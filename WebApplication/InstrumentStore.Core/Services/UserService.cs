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

		public UserService(
			InstrumentStoreDBContext dbContext,
			IJwtProvider jwtProvider)
		{
			_dbContext = dbContext;
			_jwtProvider = jwtProvider;
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
	}
}
