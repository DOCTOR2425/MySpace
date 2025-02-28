using InstrumentStore.API.Authentication;
using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.DataBase;
using InstrumentStore.Domain.Mapper;
using InstrumentStore.Domain.Service;
using InstrumentStore.Domain.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace InstrumentStore.API
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			builder.Services.AddControllers();
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			builder.Services.AddDbContext<InstrumentStoreDBContext>();

			builder.Services.AddScoped<IAdminService, AdminService>();
			builder.Services.AddScoped<IBrandService, BrandService>();
			builder.Services.AddScoped<ICartService, CartService>();
			builder.Services.AddScoped<ICityService, CityService>();
			builder.Services.AddScoped<ICountryService, CountryService>();
			builder.Services.AddScoped<IDeliveryMethodService, DeliveryMethodService>();
			builder.Services.AddScoped<IEmailService, EmailService>();
			builder.Services.AddScoped<IFillDataBaseService, FillDataBaseService>();
			builder.Services.AddScoped<IJwtProvider, JwtProvider>();
			builder.Services.AddScoped<IPaidOrderService, PaidOrderService>();
			builder.Services.AddScoped<IPaymentMethodService, PaymentMethodService>();
			builder.Services.AddScoped<IProductCategoryService, ProduCtategoryService>();
			builder.Services.AddScoped<IProductFilterService, ProductFilterService>();
			builder.Services.AddScoped<IImageService, ImageService>();
			builder.Services.AddScoped<IProductPropertyService, ProductPropertyService>();
			builder.Services.AddScoped<IProductService, ProductService>();
			builder.Services.AddScoped<IUsersService, UsersService>();

			builder.Services.AddAutoMapper(typeof(AppMappingProfile));
			builder.Services.AddScoped<CustomJwtBearerEvents>();

			builder.Services.AddAuthorization();
			builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
				.AddJwtBearer(opt =>
				{
					opt.EventsType = typeof(CustomJwtBearerEvents);

					opt.TokenValidationParameters = new()
					{
						ValidateIssuer = false,
						ValidateAudience = false,
						ValidateLifetime = true,
						ValidateIssuerSigningKey = true,
						IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtProvider.JwtKey))
					};
				});

			var app = builder.Build();

			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseCookiePolicy(new CookiePolicyOptions
			{
				MinimumSameSitePolicy = SameSiteMode.Lax,
				//HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always,
				Secure = CookieSecurePolicy.Always,
			});

			app.UseCors(x =>
			{
				x.AllowAnyOrigin();
				x.AllowAnyMethod();
				x.AllowAnyHeader();

				x.WithOrigins("https://localhost:4200")
					.AllowAnyHeader()
					.AllowAnyMethod()
					.AllowCredentials();
			});

			app.UseHttpsRedirection();
			app.UseStaticFiles();
			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();

			app.MapControllers();

			app.Run();
		}
	}
}
