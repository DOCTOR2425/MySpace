﻿using AutoMapper;
using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.DataBase;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace InstrumentStore.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class DeveloperController : ControllerBase
	{
		private readonly IDeliveryMethodService _deliveryMethodService;
		private readonly IPaymentMethodService _paymentMethodService;
		private readonly IFillDataBaseService _fillDataBaseService;
		private readonly IAdminService _adminService;
		private readonly IMapper _mapper;
		private readonly IConfiguration _config;
		private readonly InstrumentStoreDBContext _dbContext;
		private readonly IPromoCodeService _promoCodeService;

		public DeveloperController(IUserService usersService,
			IDeliveryMethodService deliveryMethodService,
			IPaymentMethodService paymentMethodService,
			IMapper mapper,
			InstrumentStoreDBContext dbContext,
			IFillDataBaseService fillDataBaseService,
			IConfiguration config,
			IAdminService adminService,
			IPromoCodeService promoCodeService)
		{
			_deliveryMethodService = deliveryMethodService;
			_paymentMethodService = paymentMethodService;
			_dbContext = dbContext;
			_fillDataBaseService = fillDataBaseService;
			_config = config;
			_adminService = adminService;
			_mapper = mapper;
			_promoCodeService = promoCodeService;
		}

		[HttpGet("create-promo-code/{promoName}")]
		public async Task<IActionResult> CreatePromoCode(string promoName)
		{
			await _promoCodeService.Create(promoName, 10);

			return Ok();
		}

		[HttpGet("FillAll")]
		public async Task<ActionResult> FillAll()
		{
			await _fillDataBaseService.FillAll();

			return Ok();
		}

		[HttpGet("ClearDataBase")]
		public async Task<ActionResult> ClearDatabase()
		{
			await _fillDataBaseService.ClearDatabase();

			return Ok();
		}

		[HttpGet("GetAdminInfo")]
		public async Task<ActionResult<string>> GetAdminInfo()
		{
			string adminInfo =
				"LoginPasswordHash:\t\t" + _config["AdminSettings:LoginPasswordHash"] + "\n" +
				"AdminMail:\t\t" + _config["AdminSettings:AdminMail"] + "\n" +
				"MailPassword:\t\t" + _config["AdminSettings:MailPassword"] + "\n" +
				"RefreshToken:\t\t" + _config["AdminSettings:RefreshToken"] + "\n" +
				"AdminId:\t\t" + _config["AdminSettings:AdminId"] + "\n";

			return adminInfo;
		}

		[HttpGet("CreateAdminInfo")]
		public async Task<ActionResult<string>> CreateAdminInfo()
		{
			Guid adminId = Guid.NewGuid();
			_config["AdminSettings:AdminId"] = adminId.ToString();

			string password = _config["AdminSettings:MailPassword"];
			_config["AdminSettings:LoginPasswordHash"] = BCrypt.Net.BCrypt.EnhancedHashPassword(password);

			var configurationFilePath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
			var jsonConfig = System.IO.File.ReadAllText(configurationFilePath);
			var jsonDocument = JsonDocument.Parse(jsonConfig);

			var jsonElement = jsonDocument.RootElement.Clone();

			using (var jsonStream = new MemoryStream())
			{
				using (var jsonWriter = new Utf8JsonWriter(jsonStream, new JsonWriterOptions { Indented = true }))
				{
					jsonWriter.WriteStartObject();

					foreach (var property in jsonElement.EnumerateObject())
					{
						if (property.Name == "AdminSettings")
						{
							jsonWriter.WritePropertyName("AdminSettings");
							jsonWriter.WriteStartObject();
							jsonWriter.WriteString("AdminId", adminId.ToString());
							jsonWriter.WriteString("MailPassword", password);
							jsonWriter.WriteString("LoginPasswordHash", _config["AdminSettings:LoginPasswordHash"]);
							jsonWriter.WriteString("RefreshToken", _config["AdminSettings:RefreshToken"]);
							jsonWriter.WriteString("AdminMail", _config["AdminSettings:AdminMail"]);
							jsonWriter.WriteEndObject();
						}
						else
						{
							property.WriteTo(jsonWriter);
						}
					}

					jsonWriter.WriteEndObject();
				}

				var newJsonConfig = Encoding.UTF8.GetString(jsonStream.ToArray());
				System.IO.File.WriteAllText(configurationFilePath, newJsonConfig);
			}

			string adminInfo = "";
			adminInfo += "AdminMail:\t\t" + _config["AdminSettings:AdminMail"] + "\n" +
						 "MailPassword:\t\t" + password + "\n" +
						 "LoginPasswordHash:\t\t" + _config["AdminSettings:LoginPasswordHash"] + "\n" +
						 "RefreshToken:\t\t" + _config["AdminSettings:RefreshToken"] + "\n" +
						 "AdminId:\t\t" + adminId.ToString() + "\n";

			return adminInfo;
		}
	}
}
