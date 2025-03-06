using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace InstrumentStore.API.Authentication
{
	public class CustomJwtBearerEvents : JwtBearerEvents
	{
		private readonly IServiceScopeFactory _scopeFactory;

		public CustomJwtBearerEvents(IServiceScopeFactory scopeFactory)
		{
			_scopeFactory = scopeFactory;
		}

		private async Task<string> UserAuthenticate(
			JwtSecurityToken token,
			AuthenticationFailedContext context)
		{
			var scope = _scopeFactory.CreateScope();
			var usersService = scope.ServiceProvider.GetRequiredService<IUsersService>();

			JwtSecurityToken oldRefreshToken = await usersService.GetRefreshToken(token);

			if (oldRefreshToken.ValidTo > DateTime.UtcNow)
			{
				return await usersService.ReLogin(token);
			}
			else
			{
				await base.AuthenticationFailed(context);
				return new JwtSecurityTokenHandler().WriteToken(token);
			}
		}

		private async Task<string> AdminAuthenticate(
			JwtSecurityToken token,
			AuthenticationFailedContext context)
		{
			var scope = _scopeFactory.CreateScope();
			var adminService = scope.ServiceProvider.GetRequiredService<IAdminService>();

			JwtSecurityToken oldRefreshToken = await adminService.GetRefreshToken(token);
			if (oldRefreshToken.ValidTo > DateTime.UtcNow)
			{
				return await adminService.ReLogin(token);
			}
			else
			{
				await base.AuthenticationFailed(context);
				return new JwtSecurityTokenHandler().WriteToken(token);
			}
		}

		public override async Task AuthenticationFailed(AuthenticationFailedContext context)
		{
			if (context.Exception is SecurityTokenExpiredException)
			{
				string strToken = context.Request.Headers["Authorization"]
				.ToString()
				.Substring("Bearer ".Length)
				.Trim();

				string newAccessToken = "";

				var handler = new JwtSecurityTokenHandler();
				JwtSecurityToken token = handler.ReadToken(strToken) as JwtSecurityToken;

				if (token.Claims.First(c => c.Type == ClaimTypes.Role)?.Value.ToLower() == "user")
					newAccessToken = await UserAuthenticate(token, context);
				else
					newAccessToken = await AdminAuthenticate(token, context);

				context.Response.Cookies.Append(JwtProvider.AccessCookiesName,
					newAccessToken,
					new CookieOptions()
					{
						Secure = true,
						SameSite = SameSiteMode.Lax,
						Expires = DateTime.Now.Add(JwtProvider.RefreshTokenLifeTime)
					});

				context.Request.Headers["Authorization"] = "Bearer " + newAccessToken;

				var newJwtToken = handler.ReadToken(newAccessToken) as JwtSecurityToken;
				var identity = new ClaimsIdentity(newJwtToken.Claims, JwtBearerDefaults.AuthenticationScheme);
				context.Principal = new ClaimsPrincipal(identity);
				context.Success();
			}
			else
			{
				Console.WriteLine(context.Exception.GetType().FullName);
				Console.WriteLine(context.Exception.Message);
				await base.AuthenticationFailed(context);
			}
		}

		public override async Task TokenValidated(TokenValidatedContext context)
		{
			await base.TokenValidated(context);
		}
	}
}
