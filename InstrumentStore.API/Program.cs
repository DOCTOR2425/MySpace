using InstrumentStore.Domain.DataBase;
using InstrumentStore.Domain.Service;
using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.Services;

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

			builder.Services.AddScoped<IProductService, ProductService>();
			builder.Services.AddScoped<IBrandService, BrandService>();
			builder.Services.AddScoped<IProductTypeService, ProductTypeService>();
			builder.Services.AddScoped<ICountryService, CountryService>();

            var app = builder.Build();

			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();
			app.UseAuthorization();
			app.MapControllers();
			
			app.UseCors(x =>
			{
				x.WithHeaders().AllowAnyHeader();
				x.WithOrigins("http://localhost:3000");//TODO Номер порта фронта
				x.WithMethods().AllowAnyMethod();
			});

			app.Run();
		}
	}
}
