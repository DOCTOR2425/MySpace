using System.Text;
using InstrumentStore.Domain.DataBase.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using InstrumentStore.Domain.Abstractions;

namespace InstrumentStore.Domain.Services
{
    public class JwtProvider : IJwtProvider// TODO Сделать что то с secret key
    {
        public static string JwtKey = "secretkeysecretkeysecretkeysecretkeysecretkeysecretkey";//6
        public static string AccessCookiesName = "token-cookies";
        public static string RefreshCookiesName = "refresh-token";
        public static int AccessTokenLifeMinets = 1;
        public static int RefreshTokenLifeDays = 30;

        public string GenerateAccessToken(User user)
        {
            Claim[] claims = [new("userId", user.UserId.ToString())];

            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtKey)), 
                    SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                signingCredentials: signingCredentials,
                expires: DateTime.UtcNow.AddMinutes(AccessTokenLifeMinets));

            return new JwtSecurityTokenHandler().WriteToken(token); ;
        }

        public string GenerateRefreshToken(User user)
        {
            Claim[] claims = [new("userId", user.UserId.ToString())];

            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtKey)),
                    SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                signingCredentials: signingCredentials,
                expires: DateTime.UtcNow.AddDays(RefreshTokenLifeDays));

            return new JwtSecurityTokenHandler().WriteToken(token); ;
        }

        public Guid GetUserIdFromToken(string token)
        {
            var sToken = new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;
            return Guid.Parse(sToken.Claims.First(claim => claim.Type == "userId").Value);
        }
    }
}
