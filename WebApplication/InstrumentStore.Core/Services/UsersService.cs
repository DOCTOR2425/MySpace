using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.Contracts.User;
using InstrumentStore.Domain.DataBase;
using InstrumentStore.Domain.DataBase.Models;
using Microsoft.EntityFrameworkCore;

namespace InstrumentStore.Domain.Services
{
    public class UsersService : IUsersService
    {
        private readonly InstrumentStoreDBContext _dbContext;
        private readonly ICityService _cityService;
        private readonly IJwtProvider _jwtProvider;
        private readonly IEmailService _emailService;

        public UsersService(
            InstrumentStoreDBContext dbContext,
            IJwtProvider jwtProvider,
            IEmailService emailService,
            ICityService cityService)
        {
            _dbContext = dbContext;
            _jwtProvider = jwtProvider;
            _emailService = emailService;
            _cityService = cityService;
        }

        public string GeneratePasswordHas(string password)
        {
            return BCrypt.Net.BCrypt.EnhancedHashPassword(password);
        }

        public async Task<bool> Verify(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.EnhancedVerify(password, passwordHash);
        }

        public async Task<Guid> Register(RegisterUserRequest registerUserRequest)
        {
            User? targetUser = await GetByEmail(registerUserRequest.Email);
            if (targetUser != null)
                throw new Exception("User with that email already exist");

            UserRegistrInfo userRegistrInfo = new UserRegistrInfo
            {
                UserRegistrInfoId = Guid.NewGuid(),
                Email = registerUserRequest.Email,
                PasswordHash = GeneratePasswordHas(registerUserRequest.Password)
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

        public async Task<string> Login(string email, string password)
        {
            User? user = await GetByEmail(email);
            if (user == null)
                throw new AuthenticationException("Нет пользователя с таким email");

            if (await Verify(password, user.UserRegistrInfo.PasswordHash) == false)
                throw new AuthenticationException("Неверный пароль");

            user.UserRegistrInfo.RefreshToken = await _jwtProvider.GenerateRefreshToken(user.UserId);
            _dbContext.SaveChanges();

            return await _jwtProvider.GenerateAccessToken(user.UserId);
        }

        public async Task<string> ReLogin(JwtSecurityToken token)
        {
            User user = await GetById(await GetUserIdFromToken(token));

            user.UserRegistrInfo.RefreshToken = await _jwtProvider.GenerateRefreshToken(user.UserId);
            await _dbContext.SaveChangesAsync();

            return await _jwtProvider.GenerateAccessToken(user.UserId);
        }

        public async Task<JwtSecurityToken> GetRefreshToken(JwtSecurityToken accessToken)
        {
            User user = await GetById(await GetUserIdFromToken(accessToken));

            return new JwtSecurityTokenHandler().ReadToken(
                user.UserRegistrInfo.RefreshToken) as JwtSecurityToken;
        }

        private async Task<Guid> GetUserIdFromToken(JwtSecurityToken token)
        {
            return Guid.Parse(token.Claims
                    .First(c => c.Type == ClaimTypes.NameIdentifier).Value);
        }

        public async Task<User?> GetByEmail(string email)
        {
            User? user = await _dbContext.User
                .Include(u => u.UserRegistrInfo)
                .FirstOrDefaultAsync(
                u => u.UserRegistrInfo.Email == email);

            return user;
        }

        public async Task<User> GetById(Guid id)
        {
            User? user = await _dbContext.User
                .Include(u => u.UserRegistrInfo)
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null)
                throw new UnauthorizedAccessException("No user with that Id");

            return user;
        }

        public async Task<Guid> RegisterUserFromOrder(RegisterUserFromOrderRequest registerUserRequest)
        {
            User? targetUser = await GetByEmail(registerUserRequest.Email);
            if (targetUser != null)
                return targetUser.UserId;

            string userPassword = GenerateLoginCode();
            UserRegistrInfo userRegistrInfo = new UserRegistrInfo
            {
                UserRegistrInfoId = Guid.NewGuid(),
                Email = registerUserRequest.Email,
                PasswordHash = GeneratePasswordHas(userPassword)
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

            SendPasswordToUser(userRegistrInfo.Email, userPassword);

            return user.UserId;
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

        private void SendPasswordToUser(string eMail, string password)
        {
            string eMailText = $"Ваш новый пароль от сервиса MySpace\n" +
                $"{password}\n" +
                $"Моежте поменять его в любое время";

            _emailService.SendMail(eMail, eMailText);
        }

        public async Task<User> GetUserFromToken(string token)
        {
            Guid userId = await _jwtProvider.GetUserIdFromToken(token);
            User user = await GetById(userId);

            return user;
        }

        public async Task<User> Update(Guid userId, UpdateUserRequest newUser)
        {
            User user = await GetById(userId);

            user.FirstName = newUser.FirstName;
            user.Surname = newUser.Surname;
            user.Telephone = newUser.Telephone;
            user.UserRegistrInfo.Email = newUser.Email;

            await _dbContext.SaveChangesAsync();
            return user;
        }

        public async Task<DeliveryAddress?> GetLastUserDeliveryAddress(Guid userId)
        {
            User user = await GetById(userId);

            List<PaidOrder> paidOrders = await _dbContext.PaidOrder
                .Include(o => o.DeliveryMethod)
                .Where(o => o.User.UserId == user.UserId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            DeliveryAddress? address = null;
            foreach (var order in paidOrders)
            {
                address = _dbContext.DeliveryAddress
                    .Include(a => a.City)
                    .FirstOrDefault(a => a.PaidOrder.PaidOrderId == order.PaidOrderId);

                if (address != null)
                    break;
            }

            return address;
        }

        public async Task<List<Product>> GetOrderedProductsPendingReviewsByUser(Guid userId)
        {
            List<Product> products = new List<Product>();

            List<Product> productsBuff = new List<Product>();
            List<PaidOrder> paidOrders = await _dbContext.PaidOrder
                .AsQueryable()
                .Where(o => o.User.UserId == userId)
                .ToListAsync();

            if (paidOrders.Any() == false)
                return products;

            foreach (var order in paidOrders)
            {
                productsBuff.AddRange(await _dbContext.PaidOrderItem
                    .AsQueryable()
                    .Include(i => i.Product)
                    .Where(i => i.PaidOrder.PaidOrderId == order.PaidOrderId)
                    .Select(i => i.Product)
                    .ToListAsync());
            }

            foreach (var product in productsBuff)
            {
                if (await _dbContext.Comment
                    .AsQueryable()
                    .Where(c => c.Product.ProductId == product.ProductId)
                    .AnyAsync() == false)
                {
                    products.Add(product);
                }
            }

            return products;
        }
    }
}
