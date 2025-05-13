using InstrumentStore.Domain.Contracts.User;
using System.IdentityModel.Tokens.Jwt;

namespace InstrumentStore.Domain.Abstractions
{
	public interface IAccountService
	{
		Task<string> LoginFirstStage(string email);
		Task<string> LoginSecondStage(string email, string code, string codeHash);
		Task<string> GenerateCodeHas(string code);
		Task<bool> VerifyCode(string token, string tokenHash);
		Task<string> AdminLoginSecondStage(string email, string code, string codeHash);
		Task<Guid> CreateUser(RegisterUserRequest registerUserRequest);
		Task<JwtSecurityToken> GetRefreshToken(JwtSecurityToken accessToken);
		Task<string> UserReLogin(JwtSecurityToken token);
		Task<string> AdminReLogin(JwtSecurityToken token);
		Task<JwtSecurityToken> GetAdminRefreshToken();
		Task<Guid> RegisterUserFromOrder(RegisterUserFromOrderRequest registerUserRequest);
	}
}