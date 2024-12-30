using System.IdentityModel.Tokens.Jwt;
using InstrumentStore.Domain.Abstractions;
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
            var scope = _scopeFactory.CreateScope();
            var usersService = scope.ServiceProvider.GetRequiredService<IUsersService>();

            if (context.Request.Path != "/login" && context.Request.Path != "/register" &&
                context.Request.Headers.ContainsKey("Authorization"))
            {
                string cookieToken = context.Request.Headers["Authorization"]
                    .ToString().Substring("Bearer ".Length).Trim();

                var token = new JwtSecurityTokenHandler().ReadToken(cookieToken) as JwtSecurityToken;

                if (token.ValidTo < DateTime.UtcNow)
                {
                    Console.WriteLine(false);

                    var oldRefreshToken = await usersService.GetRefreshToken(cookieToken);

                    if (oldRefreshToken.ValidTo > DateTime.UtcNow)
                    {
                        var newAccessToken = await usersService.ReLogin(cookieToken);
                        context.Response.Cookies.Append(JwtProvider.AccessCookiesName,
                            newAccessToken,
                            new CookieOptions()
                            {
                                Secure = true,
                                SameSite = SameSiteMode.Lax,
                                Expires = DateTime.Now.AddDays(JwtProvider.RefreshTokenLifeDays)
                            });

                        context.Request.Headers["Authorization"] = "Bearer " + newAccessToken;
                    }
                }
            }
            await _next(context);
        }
    }
}
