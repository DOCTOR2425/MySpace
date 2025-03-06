using InstrumentStore.Domain.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace InstrumentStore.Domain.Services
{
	public class JwtProvider : IJwtProvider// TODO Сделать что то с secret key
	{
		public static string JwtKey = "secretkeysecretkeysecretkeysecretkeysecretkeysecretkey";//6
		public static string AccessCookiesName = "token-cookies";
		public static string RefreshCookiesName = "refresh-token";
		public static TimeSpan AccessTokenLifeTime = TimeSpan.FromMinutes(1);
		public static TimeSpan RefreshTokenLifeTime = TimeSpan.FromDays(10);
		public static TimeSpan CookiesLifeTime = RefreshTokenLifeTime;

		private readonly IConfiguration _config;

		public JwtProvider(IConfiguration configuration)
		{
			_config = configuration;
		}

		public async Task<string> GenerateAccessToken(Guid userId)
		{
			Claim[] claims = await GenerateClaims(userId);

			var signingCredentials = new SigningCredentials(
				new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtKey)),
					SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				claims: claims,
				signingCredentials: signingCredentials,
				expires: DateTime.UtcNow.Add(AccessTokenLifeTime));

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		public async Task<string> GenerateRefreshToken(Guid userId)
		{
			Claim[] claims = await GenerateClaims(userId);

			var signingCredentials = new SigningCredentials(
				new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtKey)),
					SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				claims: claims,
				signingCredentials: signingCredentials,
				expires: DateTime.UtcNow.Add(RefreshTokenLifeTime)
				);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		private async Task<Claim[]> GenerateClaims(Guid userId)
		{
			Claim[] claims = new Claim[2];
			claims[0] = new Claim(ClaimTypes.NameIdentifier, userId.ToString());

			if (Guid.Parse(_config["AdminSettings:AdminId"]) == userId)
				claims[1] = new Claim(ClaimTypes.Role, "admin");
			else
				claims[1] = new Claim(ClaimTypes.Role, "user");

			return claims;
		}

		public async Task<Guid> GetUserIdFromToken(JwtSecurityToken token)
		{
			return Guid.Parse(token.Claims
					.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
		}

		public async Task<Guid> GetUserIdFromToken(string token)
		{
			JwtSecurityToken sToken =
				new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;

			return Guid.Parse(sToken.Claims
					.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
		}
	}
}
