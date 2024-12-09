using InstrumentStore.Domain.DataBase;
using InstrumentStore.Domain.Service;
using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.Services;
using InstrumentStore.Domain.Mapper;
//using AutoMapper;

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

            builder.Services.AddAutoMapper(typeof(AppMappingProfile));

            //builder.Services.AddAutoMapper((serviceProvider, automapper) =>
            //{
            //    using (var scope = serviceProvider.CreateScope())
            //    {
            //        var scopedProvider = scope.ServiceProvider;
            //        var countryService = scopedProvider.GetRequiredService<ICountryService>();
            //        var brandService = scopedProvider.GetRequiredService<IBrandService>();
            //        var productTypeService = scopedProvider.GetRequiredService<IProductTypeService>();

            //        automapper.AddProfile(new AppMappingProfile(countryService, brandService, productTypeService));
            //    }
            //}, typeof(AppMappingProfile));

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
                x.AllowAnyHeader();
                x.AllowAnyOrigin();
                x.AllowAnyMethod();
            });

            app.Run();
        }
    }
}
