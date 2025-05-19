using AutoMapper;
using InstrumentStore.Domain.Contracts.Cart;
using InstrumentStore.Domain.Contracts.Comment;
using InstrumentStore.Domain.Contracts.PaidOrders;
using InstrumentStore.Domain.Contracts.ProductCategories;
using InstrumentStore.Domain.Contracts.ProductProperties;
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
			CreateMap<Product, UserProductCard>();

			CreateMap<Product, AdminProductCard>()
				.ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country.Name))
				.ForMember(dest => dest.Brand, opt => opt.MapFrom(src => src.Brand.Name))
				.ForMember(dest => dest.ProductCategory, opt => opt.MapFrom(src => src.ProductCategory.Name))
				.AfterMap((src, dest, context) =>
				{
					var dbContext = (InstrumentStoreDBContext)context.Items["DbContext"];
					var image = dbContext.Image.FirstOrDefault(i => i.Product.ProductId == src.ProductId && i.Index == 0);
					dest.Image = "https://localhost:7295/images/" + image?.Name;
				});

			CreateMap<AdminProductCard, UserProductCard>();

			CreateMap<Product, ProductMinimalData>();

			CreateMap<Product, ProductData>()
				.ForPath(dest => dest.Country, opt => opt.MapFrom(src => src.Country.Name))
				.ForPath(dest => dest.Brand, opt => opt.MapFrom(src => src.Brand.Name))
				.ForPath(dest => dest.ProductCategory, opt => opt.MapFrom(src => src.ProductCategory.Name))
				.AfterMap((src, dest, context) =>
				{
					var dbContext = (InstrumentStoreDBContext)context.Items["DbContext"];
					var image = dbContext.Image.FirstOrDefault(i => i.Product.ProductId == src.ProductId && i.Index == 0);
					dest.Image = "https://localhost:7295/images/" + image?.Name;
				});

			CreateMap<CartItem, CartItemResponse>()
				.ForPath(dest => dest.ProductId, opt => opt.MapFrom(src => src.Product.ProductId))
				.ForPath(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
				.ForPath(dest => dest.ProductPrice, opt => opt.MapFrom(src => src.Product.Price))
				.ForPath(dest => dest.IsProductArchive, opt => opt.MapFrom(src => src.Product.IsArchive));

			CreateMap<User, UserOrderInfo>()
				.AfterMap((src, dest, context) =>
				{
					var dbContext = (InstrumentStoreDBContext)context.Items["DbContext"];

					PaidOrder? paidOrder = dbContext.PaidOrder
						.Include(o => o.DeliveryMethod)
						.Where(o => o.User.UserId == src.UserId)
						.OrderBy(o => o.OrderDate)
						.LastOrDefault();

					if (paidOrder == null)
						return;

					DeliveryAddress? address = dbContext.DeliveryAddress
						.FirstOrDefault(a => a.PaidOrder.PaidOrderId == paidOrder.PaidOrderId);

					if (address == null)
						return;

					dest.UserDeliveryAddress = new UserDeliveryAddress();
					dest.UserDeliveryAddress.City = address.City;
					dest.UserDeliveryAddress.Street = address.Street;
					dest.UserDeliveryAddress.HouseNumber = address.HouseNumber;
					dest.UserDeliveryAddress.Entrance = address.Entrance;
					dest.UserDeliveryAddress.Flat = address.Flat;
				});

			CreateMap<User, UserProfileResponse>();

			CreateMap<User, AdminUserResponse>();

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
								dbContext.Image.FirstOrDefault(i =>
								i.Product.ProductId == item.Product.ProductId && i.Index == 0).Name
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
								dbContext.Image.FirstOrDefault(i =>
								i.Product.ProductId == item.Product.ProductId && i.Index == 0).Name
							}
						});
					}

					dest.UserOrderInfo = context.Mapper.Map<UserOrderInfo>(src.User);
				});

			CreateMap<DeliveryAddress, UserDeliveryAddress>()
				.ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City));

			CreateMap<Product, FullProductInfoResponse>()
				.ForMember(dest => dest.ProductCategory, opt => opt.MapFrom(src => src.ProductCategory.Name))
				.ForMember(dest => dest.Brand, opt => opt.MapFrom(src => src.Brand.Name))
				.ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country.Name));

			CreateMap<ProductPropertyValue, ProductPropertyValuesResponse>()
				.ForPath(dest => dest.PropertyId, opt => opt.MapFrom(src => src.ProductProperty.ProductPropertyId))
				.ForPath(dest => dest.Name, opt => opt.MapFrom(src => src.ProductProperty.Name))
				.ForPath(dest => dest.IsRanged, opt => opt.MapFrom(src => src.ProductProperty.IsRanged));

			CreateMap<ProductProperty, ProductPropertyResponse>();
			CreateMap<ProductProperty, ProductPropertyDTOUpdate>();

			CreateMap<Comment, CommentResponse>()
				 .ForPath(dest => dest.UserName, opt => opt.MapFrom(src => src.User.FirstName));
			CreateMap<Comment, CommentForUserResponse>()
				 .ForPath(dest => dest.ProductId, opt => opt.MapFrom(src => src.Product.ProductId))
				 .ForPath(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name));

			CreateMap<ProductCategory, ProductCategoryForAdmin>();
		}
	}
}
