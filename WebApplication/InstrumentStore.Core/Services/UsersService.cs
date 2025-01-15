using InstrumentStore.Domain.Contracts.User;
using InstrumentStore.Domain.DataBase;
using InstrumentStore.Domain.DataBase.Models;
using InstrumentStore.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace InstrumentStore.Domain.Services
{
    public class UsersService : IUsersService
    {
        private readonly InstrumentStoreDBContext _dbContext;
        private readonly IJwtProvider _jwtProvider;
        private readonly IEmailService _emailService;

        public UsersService(
            InstrumentStoreDBContext dbContext,
            IJwtProvider jwtProvider,
            IEmailService emailService)
        {
            _dbContext = dbContext;
            _jwtProvider = jwtProvider;
            _emailService = emailService;
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
            User? targetUser = await GetByEMail(registerUserRequest.EMail);
            if (targetUser != null)
                throw new Exception("User with that email already exist");

            UserAdress userAdress = new UserAdress
            {
                UserAdressId = Guid.NewGuid(),
                City = registerUserRequest.City,
                Street = registerUserRequest.Street,
                HouseNumber = registerUserRequest.HouseNumber,
                Entrance = registerUserRequest.Entrance,
                Flat = registerUserRequest.Flat
            };

            UserRegistrInfo userRegistrInfo = new UserRegistrInfo
            {
                UserRegistrInfoId = Guid.NewGuid(),
                EMail = registerUserRequest.EMail,
                PasswordHash = GeneratePasswordHas(registerUserRequest.Password)
            };

            User user = new User
            {
                UserId = Guid.NewGuid(),
                FirstName = registerUserRequest.FirstName,
                Surname = registerUserRequest.Surname,
                Telephone = registerUserRequest.Telephone,
                UserAdress = userAdress,
                UserRegistrInfo = userRegistrInfo
            };

            userRegistrInfo.RefreshToken = _jwtProvider.GenerateRefreshToken(user);

            await _dbContext.UserAdresses.AddAsync(userAdress);
            await _dbContext.UserRegistrInfos.AddAsync(userRegistrInfo);
            await _dbContext.User.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            return user.UserId;
        }

        public async Task<string> Login(string email, string password)
        {
            User? user = await GetByEMail(email);
            if(user == null)
                throw new Exception("No user with that email");

            bool result = Verify(password, user.UserRegistrInfo.PasswordHash);

            if (result == false)
                throw new Exception("Invalid password of user email: " + email);

            user.UserRegistrInfo.RefreshToken = _jwtProvider.GenerateRefreshToken(user);
            _dbContext.SaveChanges();

            return _jwtProvider.GenerateAccessToken(user);
        }

        public async Task<string> ReLogin(string cookiesToken)
        {
            User user = await GetById(_jwtProvider.GetUserIdFromToken(cookiesToken));

            user.UserRegistrInfo.RefreshToken = _jwtProvider.GenerateRefreshToken(user);
            await _dbContext.SaveChangesAsync();

            return _jwtProvider.GenerateAccessToken(user);
        }

        public async Task<JwtSecurityToken> GetRefreshToken(string cookiesToken)
        {
            User user = await GetById(_jwtProvider.GetUserIdFromToken(cookiesToken));

            return new JwtSecurityTokenHandler().ReadToken(
                user.UserRegistrInfo.RefreshToken) as JwtSecurityToken;
        }

        public async Task<User?> GetByEMail(string email)
        {
            User? user = await _dbContext.User
                .Include(u => u.UserAdress)
                .Include(u => u.UserRegistrInfo)
                .FirstOrDefaultAsync(
                u => u.UserRegistrInfo.EMail == email);

            return user;
        }

        public async Task<User> GetById(Guid id)
        {
            User? user = await _dbContext.User
                .Include(u => u.UserAdress)
                .Include(u => u.UserRegistrInfo)
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null)
                throw new ArgumentNullException("No user with that Id");

            return user;
        }

        public async Task<Guid> RegisterUserFromOrder(RegisterUserFromOrderRequest registerUserRequest)
        {
            User? targetUser = await GetByEMail(registerUserRequest.EMail);
            if (targetUser != null)
                return targetUser.UserId;

            UserAdress userAdress = new UserAdress
            {
                UserAdressId = Guid.NewGuid(),
                City = registerUserRequest.City,
                Street = registerUserRequest.Street,
                HouseNumber = registerUserRequest.HouseNumber,
                Entrance = registerUserRequest.Entrance,
                Flat = registerUserRequest.Flat
            };

            string userPassword = GenerateUserPassword();
            UserRegistrInfo userRegistrInfo = new UserRegistrInfo
            {
                UserRegistrInfoId = Guid.NewGuid(),
                EMail = registerUserRequest.EMail,
                PasswordHash = GeneratePasswordHas(userPassword)
            };

            User user = new User
            {
                UserId = Guid.NewGuid(),
                FirstName = registerUserRequest.FirstName,
                Surname = registerUserRequest.Surname,
                Telephone = registerUserRequest.Telephone,
                UserAdress = userAdress,
                UserRegistrInfo = userRegistrInfo
            };

            userRegistrInfo.RefreshToken = _jwtProvider.GenerateRefreshToken(user);

            await _dbContext.UserAdresses.AddAsync(userAdress);
            await _dbContext.UserRegistrInfos.AddAsync(userRegistrInfo);
            await _dbContext.User.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            SendPasswordToUser(userRegistrInfo.EMail, userPassword);

            return user.UserId;
        }

        private string GenerateUserPassword()
        {
            int passwordLenth = 4;//TODO пароль 12
            var random = new Random();
            var result = string.Join("",
                Enumerable.Range(0, passwordLenth)
                .Select(i =>
                    random.Next(0, 10) % 2 == 0 ?
                        (char)('a' + random.Next(26)) + "" :
                        random.Next(1, 10) + "")
                );
            return result;
        }

        private void SendPasswordToUser(string eMail, string password)
        {
            string eMailText = $"Ваш новый пароль от сервиса MySpace\n" +
                $"{password}\n" +
                $"Моежте поменять его в любое время";

            _emailService.SendMail(eMail, eMailText);
        }
    }
}
