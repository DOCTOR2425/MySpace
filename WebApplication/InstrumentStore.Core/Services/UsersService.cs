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
				UserRegistrInfoId = Guid.NewGuid(),
				EMail = registerUserRequest.EMail,
				PasswordHash = GeneratePasswordHas(registerUserRequest.Password)
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

			userRegistrInfo.RefreshToken = _jwtProvider.GenerateRefreshToken(user);

			await _dbContext.UserAdresses.AddAsync(userAdress);
			await _dbContext.UserRegistrInfos.AddAsync(userRegistrInfo);
			await _dbContext.User.AddAsync(user);
			await _dbContext.SaveChangesAsync();

			return user.UserId;
		}

		public async Task<string> Login(string email, string password)
		{
			User user = await GetByEMail(email);

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
			User? user = await _dbContext.User
				.Include(u => u.UserAdress)
				.Include(u => u.UserRegistrInfo)
				.FirstOrDefaultAsync(u => u.UserId == id);

			if (user == null)
				throw new ArgumentNullException("No user with that Id");

			return user;
		}
	}
}
