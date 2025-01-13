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

        public override async Task AuthenticationFailed(AuthenticationFailedContext context)
        {
            if (context.Exception is SecurityTokenExpiredException)
            {
                var token = context.Request.Headers["Authorization"].ToString().Substring("Bearer ".Length).Trim();
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

                if (jwtToken != null)
                {
                    var scope = _scopeFactory.CreateScope();
                    var usersService = scope.ServiceProvider.GetRequiredService<IUsersService>();

                    var oldRefreshToken = await usersService.GetRefreshToken(token);

                    if (oldRefreshToken.ValidTo > DateTime.UtcNow)
                    {
                        var newAccessToken = await usersService.ReLogin(token);
                        context.Response.Cookies.Append(JwtProvider.AccessCookiesName,
                            newAccessToken,
                            new CookieOptions()
                            {
                                Secure = true,
                                SameSite = SameSiteMode.Lax,
                                Expires = DateTime.Now.AddDays(JwtProvider.RefreshTokenLifeDays)
                            });

                        context.Request.Headers["Authorization"] = "Bearer " + newAccessToken;

                        var newJwtToken = handler.ReadToken(newAccessToken) as JwtSecurityToken;
                        var identity = new ClaimsIdentity(newJwtToken.Claims, JwtBearerDefaults.AuthenticationScheme);
                        context.Principal = new ClaimsPrincipal(identity);

                        context.Success();
                    }
                }
            }
            else
            {
                await base.AuthenticationFailed(context);
            }
        }

        public override async Task TokenValidated(TokenValidatedContext context)
        {
            await base.TokenValidated(context);
        }
    }
}
