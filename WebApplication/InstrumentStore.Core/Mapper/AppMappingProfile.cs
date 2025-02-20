using AutoMapper;
using InstrumentStore.Domain.Contracts.Cart;
using InstrumentStore.Domain.Contracts.Products;
using InstrumentStore.Domain.DataBase.Models;
using InstrumentStore.Domain.Contracts.User;
using InstrumentStore.Domain.DataBase.ProcedureResultModels;

namespace InstrumentStore.Domain.Mapper
{
    public class AppMappingProfile : Profile
    {
        public AppMappingProfile()
        {
            CreateMap<Product, ProductData>()
                .ForPath(dest => dest.Country, opt => opt.MapFrom(src => src.Country.Name))
                .ForPath(dest => dest.Brand, opt => opt.MapFrom(src => src.Brand.Name))
                .ForPath(dest => dest.ProductCategory, opt => opt.MapFrom(src => src.ProductCategory.Name))
                .ForPath(dest => dest.Image, opt => opt.MapFrom(src =>
                    "https://localhost:7295/images/" + src.Image));

            CreateMap<Product, ProductCard>()
                .ForPath(dest => dest.Country, opt => opt.MapFrom(src => src.Country.Name))
                .ForPath(dest => dest.Brand, opt => opt.MapFrom(src => src.Brand.Name))
                .ForPath(dest => dest.ProductCategory, opt => opt.MapFrom(src => src.ProductCategory.Name))
                .ForPath(dest => dest.Image, opt => opt.MapFrom(src =>
                    "https://localhost:7295/images/" + src.Image));

            //CreateMap<ProductSearchResult, Product>()
            //    .ForPath(dest => dest.ProductCategory, opt => opt.MapFrom(src => new ProductCategory()))
            //    .ForPath(dest => dest.ProductCategory.ProductCategoryId, opt => opt.MapFrom(src => src.ProductCategoryId2))
            //    .ForPath(dest => dest.ProductCategory.Name, opt => opt.MapFrom(src => src.ProductCategoryName))
            //    .ForPath(dest => dest.Brand, opt => opt.MapFrom(src => new Brand()))
            //    .ForPath(dest => dest.Brand.BrandId, opt => opt.MapFrom(src => src.BrandId2))
            //    .ForPath(dest => dest.Brand.Name, opt => opt.MapFrom(src => src.BrandName))
            //    .ForPath(dest => dest.Country, opt => opt.MapFrom(src => new Country()))
            //    .ForPath(dest => dest.Country.CountryId, opt => opt.MapFrom(src => src.CountryId2))
            //    .ForPath(dest => dest.Country.Name, opt => opt.MapFrom(src => src.CountryName));


            CreateMap<CartItem, CartItemResponse>()
                .ForPath(dest => dest.Product, opt => opt.MapFrom(src => src.Product))
                .ForPath(dest => dest.Product.ProductCategory, opt => opt.MapFrom(src => src.Product.ProductCategory.Name))
                .ForPath(dest => dest.Product.Brand, opt => opt.MapFrom(src => src.Product.Brand.Name))
                .ForPath(dest => dest.Product.Country, opt => opt.MapFrom(src => src.Product.Country.Name))
                .ForPath(dest => dest.Product.Image, opt => opt.MapFrom(src =>
                    "https://localhost:7295/images/" + src.Product.Image));


            CreateMap<User, UserOrderInfo>()
               .ForCtorParam("EMail", opt => opt.MapFrom(src => src.UserRegistrInfo.EMail))
               .ForCtorParam("City", opt => opt.MapFrom(src => src.UserAdress.City))
               .ForCtorParam("Street", opt => opt.MapFrom(src => src.UserAdress.Street))
               .ForCtorParam("HouseNumber", opt => opt.MapFrom(src => src.UserAdress.HouseNumber))
               .ForCtorParam("Entrance", opt => opt.MapFrom(src => src.UserAdress.Entrance))
               .ForCtorParam("Flat", opt => opt.MapFrom(src => src.UserAdress.Flat))
               .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
               .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
               .ForMember(dest => dest.Telephone, opt => opt.MapFrom(src => src.Telephone));
        }
    }
}
