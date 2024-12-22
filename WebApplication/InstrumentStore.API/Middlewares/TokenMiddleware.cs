using System.IdentityModel.Tokens.Jwt;
using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.DataBase.Models;
using InstrumentStore.Domain.Services;

namespace InstrumentStore.API.Middlewares
{
	public class TokenMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly IServiceScopeFactory _scopeFactory;

		public TokenMiddleware(RequestDelegate next, IServiceScopeFactory scopeFactory)
		{
			_next = next;
			_scopeFactory = scopeFactory;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			using (var scope = _scopeFactory.CreateScope())
			{
				var usersService = scope.ServiceProvider.GetRequiredService<IUsersService>();
				var jwtProvider = scope.ServiceProvider.GetRequiredService<IJwtProvider>();

				try
				{
					var token = new JwtSecurityTokenHandler().ReadToken(
						context.Request.Cookies[JwtProvider.AccessCookiesName]) as JwtSecurityToken;

					if (context.Request.Path != "/login" &&
						context.Request.Path != "/register" &&
						token.ValidTo < DateTime.UtcNow)
					{
						Console.WriteLine(false);

						var refreshToken = await usersService.GetRefreshToken(
							context.Request.Cookies[JwtProvider.AccessCookiesName]);

						if (refreshToken.ValidTo > DateTime.UtcNow)
						{
							context.Response.Cookies.Append(JwtProvider.AccessCookiesName,
								await usersService.ReLogin(
									context.Request.Cookies[JwtProvider.AccessCookiesName]),
								new CookieOptions()
								{
									HttpOnly = true,
									Secure = true,
									SameSite = SameSiteMode.Lax,
									Expires = DateTime.Now.AddDays(JwtProvider.RefreshTokenLifeDays)
								});
						}
					}
					else
					{
						Console.WriteLine(true);
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
				}
				finally
				{
					await _next(context);
				}
			}
		}
	}
}
