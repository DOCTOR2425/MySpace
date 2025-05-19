using InstrumentStore.Domain.Contracts.User;
using InstrumentStore.Domain.DataBase.Models;
using System.IdentityModel.Tokens.Jwt;

namespace InstrumentStore.Domain.Abstractions
{
	public interface IUserService
	{
		Task<User?> GetByEmail(string email);
		Task<User> GetById(Guid id);
		Task<JwtSecurityToken> GetRefreshToken(JwtSecurityToken accessToken);
		Task<User> GetUserFromToken(string token);
		Task<User> Update(Guid userId, UpdateUserRequest newUser);
		Task<DeliveryAddress?> GetLastUserDeliveryAddress(Guid userId);
		Task<List<Product>> GetOrderedProductsPendingReviewsByUser(Guid userId);
		Task<List<User>> GetUsersForAdmin(
			string? query,
			DateTime? dateFrom,
			DateTime? dateTo,
			bool? isBlocked,
			bool? hasOrders);
		Task<AdminUserResponse> GetAdminUserResponse(User user);
		Task<Guid> BlockUser(Guid userId, string details);
		Task<Guid> UnblockUser(Guid userId);
	}
}