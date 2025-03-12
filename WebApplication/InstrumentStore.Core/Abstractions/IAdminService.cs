using System.IdentityModel.Tokens.Jwt;

namespace InstrumentStore.Domain.Abstractions
{
	public interface IAdminService
	{
		Task SendAdminMailAboutOrder(Guid paidOrderId);
		Task<JwtSecurityToken> GetRefreshToken();
		Task<string> ReLogin(JwtSecurityToken token);
		Task<string> Login(string email, string password);
		Task<bool> IsAdminEmail(string email);
		Task<bool> IsAdminId(Guid Id);
	}
}