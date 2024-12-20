using System.IdentityModel.Tokens.Jwt;
using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.Services;
using Microsoft.AspNetCore.Http;

namespace InstrumentStore.Domain.Middlewares
{
    public class TokenMiddleware
    {
        private readonly RequestDelegate _next;
        //private readonly IJwtProvider _jwtProvider;

        public TokenMiddleware(RequestDelegate next)
        {
            _next = next;
            //_jwtProvider = jwtProvider;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                if (Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(context.Request) != "/login" &&//https://localhost:7295/login
                    Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(context.Request) != "/register")
                {
                    var token = new JwtSecurityTokenHandler().ReadToken(
                        context.Request.Cookies[JwtProvider.AccessCookiesName]) as JwtSecurityToken;

                    Console.WriteLine(DateTime.Now.TimeOfDay.Seconds >= token.Payload.Expiration);
                    Console.WriteLine(DateTimeOffset.FromUnixTimeSeconds((long)token.Payload.Expiration).UtcDateTime
                        +"\t"+ DateTime.Now +"\n");
                }


                await this._next(context);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await this._next(context);
            }
        }
    }
}
