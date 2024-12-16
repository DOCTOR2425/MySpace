using AutoMapper;
using InstrumentStore.Domain.Contracts.Products;
using InstrumentStore.Domain.DataBase.Models;

namespace InstrumentStore.Domain.Mapper
{
    public class AppMappingProfile : Profile
    {
        public AppMappingProfile()
        {
            CreateMap<Product, ProductResponse>()
                .ForPath(dest => dest.Country, opt => opt.MapFrom(src => src.Country.Name))
                .ForPath(dest => dest.Brand, opt => opt.MapFrom(src => src.Brand.Name))
                .ForPath(dest => dest.ProductType, opt => opt.MapFrom(src => src.ProductType.Name));

            CreateMap<Product, ProductCard>()
                .ForPath(dest => dest.Country, opt => opt.MapFrom(src => src.Country.Name))
                .ForPath(dest => dest.Brand, opt => opt.MapFrom(src => src.Brand.Name))
                .ForPath(dest => dest.ProductType, opt => opt.MapFrom(src => src.ProductType.Name));

            CreateMap<CartItem, PaidOrderItem>()
                .ForPath(dest => dest.Product, opt => opt.MapFrom(src => src.Product))
                .ForPath(dest => dest.PaidOrder, opt => opt.MapFrom(src => Guid.NewGuid()));
        }
    }
}
