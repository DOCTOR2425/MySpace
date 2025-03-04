using AutoMapper;
using InstrumentStore.Domain.Contracts.Cart;
using InstrumentStore.Domain.Contracts.PaidOrders;
using InstrumentStore.Domain.Contracts.Products;
using InstrumentStore.Domain.Contracts.User;
using InstrumentStore.Domain.DataBase;
using InstrumentStore.Domain.DataBase.Models;
using Microsoft.EntityFrameworkCore;

namespace InstrumentStore.Domain.Mapper
{
	public class AppMappingProfile : Profile
	{

		public AppMappingProfile()
		{
			CreateMap<Product, ProductCard>()
				.ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country.Name))
				.ForMember(dest => dest.Brand, opt => opt.MapFrom(src => src.Brand.Name))
				.ForMember(dest => dest.ProductCategory, opt => opt.MapFrom(src => src.ProductCategory.Name))
				.AfterMap((src, dest, context) =>
				{
					var dbContext = (InstrumentStoreDBContext)context.Items["DbContext"];
					var image = dbContext.Image.FirstOrDefault(i => i.Product.ProductId == src.ProductId);
					dest.Image = "https://localhost:7295/images/" + image?.Name;
				});

			CreateMap<Product, ProductData>()
				.ForPath(dest => dest.Country, opt => opt.MapFrom(src => src.Country.Name))
				.ForPath(dest => dest.Brand, opt => opt.MapFrom(src => src.Brand.Name))
				.ForPath(dest => dest.ProductCategory, opt => opt.MapFrom(src => src.ProductCategory.Name))
				.AfterMap((src, dest, context) =>
				{
					var dbContext = (InstrumentStoreDBContext)context.Items["DbContext"];
					var image = dbContext.Image.FirstOrDefault(i => i.Product.ProductId == src.ProductId);
					dest.Image = "https://localhost:7295/images/" + image?.Name;
				});

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
				.AfterMap((src, dest, context) =>
				{
					var dbContext = (InstrumentStoreDBContext)context.Items["DbContext"];
					var image = dbContext.Image.FirstOrDefault(i => i.Product.ProductId == src.Product.ProductId);
					dest.Product.Image = "https://localhost:7295/images/" + image?.Name;
				});


			CreateMap<User, UserOrderInfo>()
				.ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.UserRegistrInfo.Email))
				.AfterMap((src, dest, context) =>
				{
					var dbContext = (InstrumentStoreDBContext)context.Items["DbContext"];

					UserAddress? address = dbContext.UserAddress
						.Include(a => a.City)
						.FirstOrDefault(a => a.User.UserId == src.UserId);
					if (address == null)
						return;

					dest.City = address.City.Name;
					dest.Street = address.Street;
					dest.HouseNumber = address.HouseNumber;
					dest.Entrance = address.Entrance;
					dest.Flat = address.Flat;
				});

			CreateMap<User, UserProfileResponse>()
				.ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.UserRegistrInfo.Email))
				.AfterMap((src, dest, context) =>
				{
					var dbContext = (InstrumentStoreDBContext)context.Items["DbContext"];

					UserAddress? address = dbContext.UserAddress
						.Include(a => a.City)
						.FirstOrDefault(a => a.User.UserId == src.UserId);
					if (address == null)
						return;

					dest.City = address.City.Name;
					dest.Street = address.Street;
					dest.HouseNumber = address.HouseNumber;
					dest.Entrance = address.Entrance;
					dest.Flat = address.Flat;
				});

			CreateMap<PaidOrder, UserPaidOrderResponse>()
				.AfterMap((src, dest, context) =>
				{
					var dbContext = (InstrumentStoreDBContext)context.Items["DbContext"];

					dest.PaidOrderItems = new List<PaidOrderItemResponse>();

					List<PaidOrderItem> paidOrderItems = dbContext.PaidOrderItem
						.Include(i => i.Product)
						.Include(i => i.Product.Brand)
						.Include(i => i.Product.Country)
						.Include(i => i.Product.ProductCategory)
						.Where(i => i.PaidOrder.PaidOrderId == src.PaidOrderId)
						.ToList();

					foreach (var item in paidOrderItems)
					{
						dest.PaidOrderItems.Add(new PaidOrderItemResponse()
						{
							PaidOrderItemId = item.PaidOrderItemId,
							Price = item.Price,
							Quantity = item.Quantity,

							ProductData = new ProductData()
							{
								ProductId = item.Product.ProductId,
								Name = item.Product.Name,
								Description = item.Product.Description,
								Brand = item.Product.Brand.Name,
								Country = item.Product.Country.Name,
								Price = item.Product.Price,
								Quantity = item.Quantity,
								ProductCategory = item.Product.ProductCategory.Name,

								Image = "https://localhost:7295/images/" +
								dbContext.Image.First(i =>
								i.Product.ProductId == item.Product.ProductId).Name
							}
						});
					}
				});

			CreateMap<PaidOrder, AdminPaidOrderResponse>()
				.AfterMap((src, dest, context) =>
				{
					var dbContext = (InstrumentStoreDBContext)context.Items["DbContext"];

					dest.PaidOrderItems = new List<PaidOrderItemResponse>();

					List<PaidOrderItem> paidOrderItems = dbContext.PaidOrderItem
						.Include(i => i.Product)
						.Include(i => i.Product.Brand)
						.Include(i => i.Product.Country)
						.Include(i => i.Product.ProductCategory)
						.Where(i => i.PaidOrder.PaidOrderId == src.PaidOrderId)
						.ToList();

					foreach (var item in paidOrderItems)
					{
						dest.PaidOrderItems.Add(new PaidOrderItemResponse()
						{
							PaidOrderItemId = item.PaidOrderItemId,
							Price = item.Price,
							Quantity = item.Quantity,

							ProductData = new ProductData()
							{
								ProductId = item.Product.ProductId,
								Name = item.Product.Name,
								Description = item.Product.Description,
								Brand = item.Product.Brand.Name,
								Country = item.Product.Country.Name,
								Price = item.Product.Price,
								Quantity = item.Quantity,
								ProductCategory = item.Product.ProductCategory.Name,

								Image = "https://localhost:7295/images/" +
								dbContext.Image.First(i =>
								i.Product.ProductId == item.Product.ProductId).Name
							}
						});
					}

					dest.UserOrderInfo = context.Mapper.Map<UserOrderInfo>(src.User);
				});
		}
	}
}
