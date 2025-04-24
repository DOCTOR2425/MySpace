using System.Security.Authentication;
using AutoMapper;
using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.Contracts.User;
using InstrumentStore.Domain.DataBase;
using InstrumentStore.Domain.DataBase.Models;
using Microsoft.Extensions.Configuration;

namespace InstrumentStore.Domain.Services
{
    public class AccountService : IAccountService
    {
        private readonly InstrumentStoreDBContext _dbContext;
        private readonly IUsersService _usersService;
        private readonly IEmailService _emailService;
        private readonly IAdminService _adminService;
        private readonly IMapper _mapper;
        private readonly IJwtProvider _jwtProvider;
        private readonly IConfiguration _config;

        public static string LoginToken { get; } = "login-token";

        public AccountService(
            IUsersService usersService,
            IMapper mapper,
            InstrumentStoreDBContext dbContext,
            IEmailService emailService,
            IJwtProvider jwtProvider,
            IAdminService adminService,
            IConfiguration configuration)
        {
            _usersService = usersService;
            _mapper = mapper;
            _dbContext = dbContext;
            _emailService = emailService;
            _jwtProvider = jwtProvider;
            _adminService = adminService;
            _config = configuration;
        }

        public async Task<string> LoginFirstStage(string email)
        {
            User? user = await _usersService.GetByEmail(email);
            if (user == null)
                throw new AuthenticationException("Нет пользователя с таким email");

            string code = GenerateLoginCode();
            _emailService.SendMail(email, code);
            return code;
        }

        private string GenerateLoginCode()
        {
            int passwordLenth = 8;
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

        public async Task<string> GenerateCodeHas(string code)
        {
            return BCrypt.Net.BCrypt.EnhancedHashPassword(code);
        }

        public async Task<bool> VerifyCode(string code, string codeHash)
        {
            return BCrypt.Net.BCrypt.EnhancedVerify(code, codeHash);
        }

        public async Task<string> LoginSecondStage(string email, string code, string codeHash)
        {
            User? user = await _usersService.GetByEmail(email);
            if (user == null)
                throw new AuthenticationException("Нет пользователя с таким email");

            if (await VerifyCode(email + code, codeHash) == false)
                throw new AuthenticationException("Неверный код");

            user.UserRegistrInfo.RefreshToken = await _jwtProvider.GenerateRefreshToken(user.UserId);
            _dbContext.SaveChanges();

            return await _jwtProvider.GenerateAccessToken(user.UserId);
        }

        public async Task<string> AdminLoginSecondStage(string email, string code, string codeHash)
        {
            if (await _adminService.IsAdminEmail(email) == false)
                throw new Exception("Invalid admin email");

            if (await VerifyCode(email + code, codeHash) == false)
                throw new Exception("Invalid password of admin, email: " + email);

            Guid adminId = Guid.Parse(_config["AdminSettings:AdminId"]);

            _config["AdminSettings:RefreshToken"] =
                await _jwtProvider.GenerateRefreshToken(adminId);

            return await _jwtProvider.GenerateAccessToken(adminId);
        }

        public async Task<Guid> Register(RegisterUserRequest registerUserRequest)
        {
            User? targetUser = await _usersService.GetByEmail(registerUserRequest.Email);
            if (targetUser != null)
                throw new Exception("User with that email already exist");

            UserRegistrInfo userRegistrInfo = new UserRegistrInfo
            {
                UserRegistrInfoId = Guid.NewGuid(),
                Email = registerUserRequest.Email
            };

            User user = new User
            {
                UserId = Guid.NewGuid(),
                FirstName = registerUserRequest.FirstName,
                Surname = registerUserRequest.Surname,
                Telephone = registerUserRequest.Telephone,
                UserRegistrInfo = userRegistrInfo
            };

            userRegistrInfo.RefreshToken = await _jwtProvider.GenerateRefreshToken(user.UserId);

            await _dbContext.UserRegistrInfo.AddAsync(userRegistrInfo);
            await _dbContext.User.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            return user.UserId;
        }
    }
}
