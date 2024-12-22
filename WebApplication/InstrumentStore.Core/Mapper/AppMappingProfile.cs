using AutoMapper;
using InstrumentStore.Domain.Contracts.Cart;
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


			CreateMap<CartItem, CartItemResponse>()
				.ForPath(dest => dest.Product, opt => opt.MapFrom(src => src.Product))
				.ForPath(dest => dest.Product.ProductType, opt => opt.MapFrom(src => src.Product.ProductType.Name))
				.ForPath(dest => dest.Product.Brand, opt => opt.MapFrom(src => src.Product.Brand.Name))
				.ForPath(dest => dest.Product.Country, opt => opt.MapFrom(src => src.Product.Country.Name));
		}
	}
}
