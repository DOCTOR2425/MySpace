using AutoMapper;
using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.Contracts.User;
using InstrumentStore.Domain.DataBase;
using InstrumentStore.Domain.DataBase.Models;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;

namespace InstrumentStore.Domain.Services
{
	public class AccountService : IAccountService
	{
		private readonly InstrumentStoreDBContext _dbContext;
		private readonly IUserService _usersService;
		private readonly IEmailService _emailService;
		private readonly IAdminService _adminService;
		private readonly IMapper _mapper;
		private readonly IJwtProvider _jwtProvider;
		private readonly IConfiguration _config;

		public static string LoginCode { get; } = "login-code";

		public AccountService(
			IUserService usersService,
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
			if (user == null && email != _config["AdminSettings:AdminMail"])
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

			user.RefreshToken = await _jwtProvider.GenerateRefreshToken(user.UserId);
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

		public async Task<Guid> CreateUser(RegisterUserRequest registerUserRequest)
		{
			User? targetUser = await _usersService.GetByEmail(registerUserRequest.Email);
			if (targetUser != null)
				throw new AuthenticationException("Пользователь с таким Email уже существует");

			User user = new User
			{
				UserId = Guid.NewGuid(),
				FirstName = registerUserRequest.FirstName,
				Surname = registerUserRequest.Surname,
				Telephone = registerUserRequest.Telephone,
				Email = registerUserRequest.Email,
				RegistrationDate = DateTime.Now,
			};

			user.RefreshToken = await _jwtProvider.GenerateRefreshToken(user.UserId);

			await _dbContext.User.AddAsync(user);
			await _dbContext.SaveChangesAsync();

			return user.UserId;
		}

		public async Task<string> UserReLogin(JwtSecurityToken token)
		{
			User user = await _usersService.GetById(await _jwtProvider.GetUserIdFromToken(token));

			user.RefreshToken = await _jwtProvider.GenerateRefreshToken(user.UserId);
			await _dbContext.SaveChangesAsync();

			return await _jwtProvider.GenerateAccessToken(user.UserId);
		}

		public async Task<string> AdminReLogin(JwtSecurityToken token)
		{
			Guid adminId = await _jwtProvider.GetUserIdFromToken(token);

			return await _jwtProvider.GenerateAccessToken(adminId);
		}

		public async Task<JwtSecurityToken> GetRefreshToken(JwtSecurityToken accessToken)
		{
			User user = await _usersService.GetById(await _jwtProvider.GetUserIdFromToken(accessToken));

			return new JwtSecurityTokenHandler().ReadToken(
				user.RefreshToken) as JwtSecurityToken;
		}

		public async Task<JwtSecurityToken> GetAdminRefreshToken()
		{
			return new JwtSecurityTokenHandler()
				.ReadToken(_config["AdminSettings:RefreshToken"]) as JwtSecurityToken;
		}

		public async Task<Guid> RegisterUserFromOrder(RegisterUserFromOrderRequest registerUserRequest)
		{
			User? targetUser = await _usersService.GetByEmail(registerUserRequest.Email);
			if (targetUser != null)
				return targetUser.UserId;

			User user = new User
			{
				UserId = Guid.NewGuid(),
				FirstName = registerUserRequest.FirstName,
				Surname = registerUserRequest.Surname,
				Telephone = registerUserRequest.Telephone,
				Email = registerUserRequest.Email,
				RegistrationDate = DateTime.Now
			};

			user.RefreshToken = await _jwtProvider.GenerateRefreshToken(user.UserId);

			await _dbContext.User.AddAsync(user);
			await _dbContext.SaveChangesAsync();

			return user.UserId;
		}
	}
}
