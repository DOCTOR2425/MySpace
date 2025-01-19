using System.IdentityModel.Tokens.Jwt;

namespace InstrumentStore.Domain.Abstractions
{
	public interface IJwtProvider
	{
		Task<string> GenerateAccessToken(Guid userId);
		Task<string> GenerateRefreshToken(Guid userId);
		Task<Guid> GetUserIdFromToken(string token);
		Task<Guid> GetUserIdFromToken(JwtSecurityToken token);
	}
}