using AutoMapper;
using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.Contracts.Cart;
using InstrumentStore.Domain.Contracts.Products;
using InstrumentStore.Domain.DataBase;
using InstrumentStore.Domain.DataBase.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Authentication;

namespace InstrumentStore.Domain.Services
{
	public class CartService : ICartService
	{
		private readonly InstrumentStoreDBContext _dbContext;
		private readonly IUserService _usersService;
		private readonly IProductService _productService;
		private readonly IPaidOrderService _paidOrderService;
		private readonly IAdminService _adminService;
		private readonly IMapper _mapper;

		public CartService(InstrumentStoreDBContext dbContext,
			IUserService usersService,
			IProductService productService,
			IPaidOrderService paidOrderService,
			IAdminService adminService,
			IMapper mapper)
		{
			_dbContext = dbContext;
			_usersService = usersService;
			_productService = productService;
			_paidOrderService = paidOrderService;
			_adminService = adminService;
			_mapper = mapper;
		}

		public async Task<List<CartItem>> GetUserCartItems(Guid userId)
		{
			return await _dbContext.CartItem
				.Include(c => c.User)
				.Include(c => c.Product)
				.Include(c => c.Product.Brand)
				.Include(c => c.Product.Country)
				.Include(c => c.Product.ProductCategory)
				.Where(c => c.User.UserId == userId)
				.AsNoTracking()
				.ToListAsync();
		}

		public async Task<List<PaidOrderItem>> GetUserOrders(Guid userId)
		{
			return await _dbContext.PaidOrderItem
				.Include(i => i.Product)
				.Include(i => i.PaidOrder)
				.Where(i => i.PaidOrder.User.UserId == userId)
				.AsNoTracking()
				.ToListAsync();
		}

		public async Task<Guid> AddToCart(Guid userId, Guid productId, int quantity)
		{
			CartItem? targetCartItem = await _dbContext.CartItem
				.Include(c => c.User)
				.Include(c => c.Product)
				.FirstOrDefaultAsync(c =>
					c.User.UserId == userId &&
					c.Product.ProductId == productId);

			if (targetCartItem != null)
				return await ChangeItemQuantity(targetCartItem.CartItemId, quantity);

			CartItem cartItem = new CartItem()
			{
				CartItemId = Guid.NewGuid(),
				User = await _usersService.GetById(userId),
				Product = await _productService.GetById(productId),
				Quantity = quantity
			};

			await _dbContext.CartItem.AddAsync(cartItem);
			await _dbContext.SaveChangesAsync();

			return cartItem.CartItemId;
		}

		public async Task<Guid> RemoveFromCart(Guid productId)
		{
			await _dbContext.CartItem
				.Where(c => c.Product.ProductId == productId)
				.ExecuteDeleteAsync();

			return productId;
		}

		public async Task<Guid> OrderCartForRegistered(Guid userId, OrderRequest orderCartRequest)
		{
			User user = await _usersService.GetById(userId);
			if (user.BlockDate != null)
				throw new AuthenticationException("Запрещено оформлять заказы по причине бана");
			if ((await GetUserCartItems(userId)).Any() == false)
				throw new ArgumentNullException("В корзине нет ни одного товара");

			foreach (var item in await GetUserCartItems(userId))
			{
				if (item.Quantity > item.Product.Quantity + 5)
					throw new InvalidOperationException("more goods then we have");
			}

			Guid paidOrderId = await _paidOrderService.Create(userId, orderCartRequest);

			await _adminService.SendAdminMailAboutOrder(paidOrderId);

			return paidOrderId;
		}

		public async Task<Guid> OrderCartForUnregistered(Guid userId, OrderCartOfUnregisteredRequest request)
		{
			foreach (var item in request.CartItems)
				await AddToCart(userId, item.ProductId, item.Quantity);

			OrderRequest orderCartRequest = new OrderRequest()
			{
				DeliveryMethodId = request.DeliveryMethodId,
				PaymentMethod = request.PaymentMethod,
				UserDelivaryAddress = request.UserDelivaryAddress
			};

			return await OrderCartForRegistered(userId, orderCartRequest);
		}

		public async Task<Guid> OrderProduct(Guid userId,
			Guid productId,
			int quantity,
			OrderRequest orderRequest)
		{
			Guid paidOrderId = await _paidOrderService.Create(userId, orderRequest);

			await _dbContext.PaidOrderItem.AddAsync(new PaidOrderItem()
			{
				PaidOrderItemId = Guid.NewGuid(),
				PaidOrder = await _paidOrderService.GetById(paidOrderId),
				Product = await _productService.GetById(productId),
				Quantity = quantity,
			});

			await _dbContext.SaveChangesAsync();

			await _adminService.SendAdminMailAboutOrder(paidOrderId);

			return paidOrderId;
		}

		private async Task<Guid> ChangeItemQuantity(Guid cartItemId, int quantity)
		{
			if (quantity == 0)
			{
				await _dbContext.CartItem
					.Where(i => i.CartItemId == cartItemId)
					.ExecuteDeleteAsync();

				return cartItemId;
			}

			await _dbContext.CartItem
				.Where(i => i.CartItemId == cartItemId)
				.ExecuteUpdateAsync(i => i.SetProperty(i => i.Quantity, quantity));

			return cartItemId;
		}

		public async Task<int> GetProductQuantityInUserCart(Guid productId, Guid userId)
		{
			CartItem? target = await _dbContext.CartItem
				.FirstOrDefaultAsync(i => i.User.UserId == userId &&
					i.Product.ProductId == productId);

			if (target == null)
				return 0;
			return target.Quantity;
		}

		public async Task<CartItemResponse> GetCartItemResponse(CartItem cartItem)
		{
			CartItemResponse response = _mapper.Map<CartItemResponse>(cartItem);

			Image? image = await _dbContext.Image
				.FirstOrDefaultAsync(i => i.Product.ProductId == cartItem.Product.ProductId &&
					i.Index == 0);
			response.ProductImage = "https://localhost:7295/images/" + image?.Name;

			return response;
		}

		public async Task<List<ProductMinimalData>> GetProductForUnregestereCart(List<Guid> productsId)
		{
			List<ProductMinimalData> products = new List<ProductMinimalData>(productsId.Count);
			foreach (Guid productId in productsId)
				products.Add(await _productService.GetProductMinimalData(productId));

			return products;
		}
	}
}
