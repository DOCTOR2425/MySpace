using AutoMapper;
using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.DataBase;
using InstrumentStore.Domain.DataBase.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace InstrumentStore.Domain.Services
{
	public class AdminService : IAdminService
	{
		private readonly InstrumentStoreDBContext _dbContext;
		private readonly IUsersService _usersService;
		private readonly IProductService _productService;
		private readonly IPaidOrderService _paidOrderService;
		private readonly IDeliveryMethodService _deliveryMethodService;
		private readonly IPaymentMethodService _paymentMethodService;
		private readonly IEmailService _emailService;
		private readonly IJwtProvider _jwtProvider;
		private readonly IMapper _mapper;
		private readonly IConfiguration _config;

		public AdminService(InstrumentStoreDBContext dbContext,
			IUsersService usersService,
			IProductService productService,
			IPaidOrderService paidOrderService,
			IDeliveryMethodService eliveryMethodService,
			IPaymentMethodService paymentMethodService,
			IMapper mapper,
			IEmailService emailService,
			IConfiguration config,
			IJwtProvider jwtProvider)
		{
			_dbContext = dbContext;
			_usersService = usersService;
			_productService = productService;
			_paidOrderService = paidOrderService;
			_deliveryMethodService = eliveryMethodService;
			_paymentMethodService = paymentMethodService;
			_mapper = mapper;
			_emailService = emailService;
			_config = config;
			_jwtProvider = jwtProvider;
		}

		public async Task SendAdminMailAboutOrder(Guid paidOrderId)
		{
			PaidOrder paidOrder = await _paidOrderService.GetById(paidOrderId);
			List<PaidOrderItem> paidOrderItems = await _paidOrderService.GetAllItemsByOrder(paidOrderId);

			string mailText = string.Empty;

			mailText += $"Заказ от {paidOrder.OrderDate}\n";
			mailText += $"Клиент - {paidOrder.User.Surname} {paidOrder.User.FirstName}\n";
			mailText += $"Телефон клиента - {paidOrder.User.Telephone}\n";
			mailText += $"Email клиента - {paidOrder.User.UserRegistrInfo.Email}\n";
			mailText += $"Способ оплаты - {paidOrder.PaymentMethod}\n";
			mailText += $"Способ доставки - {paidOrder.DeliveryMethod.Name} (стоимость - {paidOrder.DeliveryMethod.Price})\n";

			DeliveryAddress? deliveryAddress = await _paidOrderService.GetDeliveryAddressByOrderId(paidOrderId);

			if (deliveryAddress != null &&
				await _deliveryMethodService.IsHomeDelivery(paidOrder.DeliveryMethod.DeliveryMethodId))
				mailText += $"Адрес клиента - {deliveryAddress.ToString()}\n";

			mailText += "\n\n   ---- Заказанные товары ----\n";
			decimal summaryPrice = 0;

			foreach (var item in paidOrderItems)
			{
				summaryPrice += item.Product.Price * item.Quantity;

				mailText += $"{item.Product.Name} x {item.Quantity}  -  " +
					$"{item.Product.Price * item.Quantity}р.\n";
			}

			mailText += $"\nОбщая сумма заказанных товаров: {summaryPrice}.р";
			mailText += $"\nОбщая сумма с доставкой: {summaryPrice + paidOrder.DeliveryMethod.Price}.р";

			_emailService.SendMail(_config["AdminSettings:AdminMail"], mailText, "Новый заказ");
		}

		public async Task<JwtSecurityToken> GetRefreshToken(JwtSecurityToken token)
		{
			return new JwtSecurityTokenHandler()
				.ReadToken(_config["AdminSettings:RefreshToken"]) as JwtSecurityToken;
		}

		public async Task<string> Login(string email, string password)
		{
			if (await IsAdminEmail(email) == false)
				throw new Exception("Invalid admin email");

			if (await Verify(password, _config["AdminSettings:LoginPasswordHash"]) == false)
				throw new Exception("Invalid password of admin, email: " + email);

			Guid adminId = Guid.Parse(_config["AdminSettings:AdminId"]);

			_config["AdminSettings:RefreshToken"] =
				await _jwtProvider.GenerateRefreshToken(adminId);

			return await _jwtProvider.GenerateAccessToken(adminId);
		}

		public async Task<bool> Verify(string password, string passwordHash)
		{
			return BCrypt.Net.BCrypt.EnhancedVerify(password, passwordHash);
		}

		public async Task<string> ReLogin(JwtSecurityToken token)
		{
			Guid adminId = await GetUserIdFromToken(token);

			_config["AdminSettings:RefreshToken"] = await _jwtProvider.GenerateRefreshToken(adminId);

			return await _jwtProvider.GenerateAccessToken(adminId);
		}

		private async Task<Guid> GetUserIdFromToken(JwtSecurityToken token)
		{
			return Guid.Parse(token.Claims
					.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
		}

		public async Task<bool> IsAdminEmail(string email)
		{
			return _config["AdminSettings:AdminMail"] == email;
		}

		public async Task<bool> IsAdminId(Guid Id)
		{
			return Guid.Parse(_config["AdminSettings:AdminId"]) == Id;
		}
	}
}
