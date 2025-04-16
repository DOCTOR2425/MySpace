using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.Contracts.Cart;
using InstrumentStore.Domain.DataBase;
using InstrumentStore.Domain.DataBase.Models;
using Microsoft.EntityFrameworkCore;

namespace InstrumentStore.Domain.Services
{
    public class CartService : ICartService
    {
        private readonly InstrumentStoreDBContext _dbContext;
        private readonly IUsersService _usersService;
        private readonly IProductService _productService;
        private readonly IPaidOrderService _paidOrderService;
        private readonly IAdminService _adminService;

        public CartService(InstrumentStoreDBContext dbContext,
            IUsersService usersService,
            IProductService productService,
            IPaidOrderService paidOrderService,
            IAdminService adminService)
        {
            _dbContext = dbContext;
            _usersService = usersService;
            _productService = productService;
            _paidOrderService = paidOrderService;
            _adminService = adminService;
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

        public async Task<List<PaidOrderItem>> GetAllOrders(Guid userId)
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

        public async Task<Guid> RemoveFromCart(Guid cartItemId)
        {
            await _dbContext.CartItem
                .Where(c => c.CartItemId == cartItemId)
                .ExecuteDeleteAsync();

            return cartItemId;
        }

        public async Task<Guid> OrderCartForRegistered(Guid userId, OrderRequest orderCartRequest)
        {
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
            (await _dbContext.CartItem.FindAsync(cartItemId)).Quantity = quantity;

            await _dbContext.SaveChangesAsync();

            return cartItemId;
        }
    }
}
