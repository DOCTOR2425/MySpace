using AutoMapper;
using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.Contracts.Products;
using InstrumentStore.Domain.DataBase.Models;
using InstrumentStore.Domain.Services;

namespace InstrumentStore.Domain.Mapper
{
    public class AppMappingProfile : Profile
    {
        ICountryService _countryService;
        IBrandService _brandService;
        IProductTypeService _productTypeService;

        public AppMappingProfile(ICountryService countryService,
            IBrandService brandService, IProductTypeService productTypeService)
        {
            _countryService = countryService;
            _brandService = brandService;
            _productTypeService = productTypeService;


            CreateMap<Product, ProductResponse>()
                .ForPath(dest => dest.Country, opt => opt.MapFrom(src => src.Country.Name))
                .ForPath(dest => dest.Brand, opt => opt.MapFrom(src => src.Brand.Name))
                .ForPath(dest => dest.ProductType, opt => opt.MapFrom(src => src.ProductType.Name));

            CreateMap<Product, ProductCard>()
                .ForPath(dest => dest.Country, opt => opt.MapFrom(src => src.Country.Name))
                .ForPath(dest => dest.Brand, opt => opt.MapFrom(src => src.Brand.Name))
                .ForPath(dest => dest.ProductType, opt => opt.MapFrom(src => src.ProductType.Name));

            CreateMap<ProductRequest, Product>()
                .ForPath(dest => dest.Country, opt => opt.MapFrom(src => _countryService.GetById(src.CountryId).Result))
                .ForPath(dest => dest.Brand, opt => opt.MapFrom(src => _brandService.GetById(src.BrandId).Result))
                .ForPath(dest => dest.ProductType, opt => opt.MapFrom(src => _productTypeService.GetById(src.ProductTypeId).Result));
        }
    }
}
/*
{
  "name": "CheckRequest",
  "description": "И ещё ForPath",
  "price": 30,
  "quantity": 30,
  "image": "QA==",
  "productTypeId": "dd6995d4-30fe-4637-a34d-1f7d8ef2f53b",
  "brandId": "f0c2afe2-70d3-4a4f-b9c9-490a8020730f",
  "countryId": "7a0e0584-7c19-403f-bf9f-20753e9cd175"
}
*/