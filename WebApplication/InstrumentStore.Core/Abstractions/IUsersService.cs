using System.IdentityModel.Tokens.Jwt;
using InstrumentStore.Domain.Contracts.User;
using InstrumentStore.Domain.DataBase.Models;

namespace InstrumentStore.Domain.Abstractions
{
    public interface IUsersService
    {
        string GeneratePasswordHas(string password);
        Task<User?> GetByEmail(string email);
        Task<string> Login(string email, string password);
        Task<Guid> Register(RegisterUserRequest registerUserRequest);
        Task<bool> Verify(string password, string passwordHash);
        Task<User> GetById(Guid id);
        Task<string> ReLogin(JwtSecurityToken token);
        Task<JwtSecurityToken> GetRefreshToken(JwtSecurityToken accessToken);
        Task<User> GetUserFromToken(string token);
        Task<Guid> RegisterUserFromOrder(RegisterUserFromOrderRequest registerUserRequest);
        Task<User> Update(Guid userId, UpdateUserRequest newUser);
        Task<DeliveryAddress?> GetLastUserDeliveryAddress(Guid userId);
        Task<List<Product>> GetOrderedProductsPendingReviewsByUser(Guid userId);
    }
}