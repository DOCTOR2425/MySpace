using InstrumentStore.Domain.Contracts.Users;
using InstrumentStore.Domain.DataBase;
using InstrumentStore.Domain.DataBase.Models;
using InstrumentStore.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace InstrumentStore.Domain.Services
{
    public class UsersService : IUsersService
    {
        private readonly InstrumentStoreDBContext _dbContext;
        private readonly IJwtProvider _jwtProvider;

        public UsersService(InstrumentStoreDBContext dbContext, IJwtProvider jwtProvider)
        {
            _dbContext = dbContext;
            _jwtProvider = jwtProvider;
        }
        public string GeneratePasswordHas(string password)
        {
            return BCrypt.Net.BCrypt.EnhancedHashPassword(password);
        }

        public bool Verify(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.EnhancedVerify(password, passwordHash);
        }

        public async Task<Guid> Register(RegisterUserRequest registerUserRequest)
        {
            string passwordHash = GeneratePasswordHas(registerUserRequest.Password);

            UserAdress userAdress = new UserAdress
            {
                UserAdressId = Guid.NewGuid(),
                City = registerUserRequest.City,
                Entrance = registerUserRequest.Entrance,
                Street = registerUserRequest.Street,
                Flat = registerUserRequest.Flat
            };

            UserRegistrInfo userRegistrInfo = new UserRegistrInfo
            {
                EMail = registerUserRequest.EMail,
                PasswordHash = passwordHash
            };

            User user = new User
            {
                UserId = Guid.NewGuid(),
                FirstName = registerUserRequest.FirstName,
                Surname = registerUserRequest.Surname,
                Patronymic = registerUserRequest.Patronymic,
                Telephone = registerUserRequest.Telephone,
                UserAdress = userAdress,
                UserRegistrInfo = userRegistrInfo
            };

            await _dbContext.UserAdresses.AddAsync(userAdress);
            await _dbContext.UserRegistrInfos.AddAsync(userRegistrInfo);
            await _dbContext.User.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            return user.UserId;
        }

        public async Task<string[]> Login(string email, string password)
        {
            User user = await GetByEMail(email);

            bool result = Verify(password, user.UserRegistrInfo.PasswordHash);

            if (result == false)
                throw new Exception("Invalid password of user email: " + email);

            string[] tokens = new string[] {
                _jwtProvider.GenerateAccessToken(user),
                _jwtProvider.GenerateRefreshToken(user)
            };

            return tokens;
        }

        public async Task<User> GetByEMail(string email)
        {
            User? user = await _dbContext.User
                .Include(u => u.UserAdress)
                .Include(u => u.UserRegistrInfo)
                .FirstOrDefaultAsync(
                u => u.UserRegistrInfo.EMail == email);

            if (user == null)
                throw new ArgumentNullException("No user with that email");

            return user;
        }

        public async Task<User> GetById(Guid id)
        {
            return await _dbContext.User
                .Include(u => u.UserAdress)
                .Include(u => u.UserRegistrInfo)
                .Where(u => u.UserId == id)
                .FirstOrDefaultAsync();
        }

        public void InsertTokenInCookies(HttpContext context, string[] tokens)
        {
            context.Response.Cookies.Append(JwtProvider.AccessCookiesName, tokens[0], new CookieOptions()
            {
                Expires = DateTime.Now.AddMinutes(JwtProvider.AccessTokenLifeMinets)
            });
            context.Response.Cookies.Append(JwtProvider.RefreshCookiesName, tokens[1], new CookieOptions()
            {
                Expires = DateTime.Now.AddMinutes(JwtProvider.RefreshTokenLifeDays)
            });
        }
    }
}
