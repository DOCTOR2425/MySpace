using AutoMapper;
using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.DataBase;
using InstrumentStore.Domain.DataBase.Models;
using Microsoft.EntityFrameworkCore;

namespace InstrumentStore.Domain.Services
{
    public class CartService
    {
        private InstrumentStoreDBContext _dbContext;
        private IUsersService _usersService;
        private IProductService _productService;
        private IDeliveryMethodService _deliveryMethodService;
        private IPaymentMethodService _paymentMethodService;
        private IPaidOrderService _paidOrderService;
        private IMapper _mapper;

        public CartService(InstrumentStoreDBContext dbContext, 
            IUsersService usersService, 
            IProductService productService, 
            IDeliveryMethodService deliveryMethodService, 
            IPaymentMethodService paymentMethodService,
            IPaidOrderService paidOrderService,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _usersService = usersService;
            _productService = productService;
            _deliveryMethodService = deliveryMethodService;
            _paymentMethodService = paymentMethodService;
            _paidOrderService = paidOrderService;
            _mapper = mapper;
        }

        public async Task<List<CartItem>> GetAll(Guid userId)
        {
            return await _dbContext.CartItem
                .Include(c => c.User)
                .Include(c => c.Product)
                .Where(c => c.User.UserId == userId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async void AddToCart(Guid userId, Guid productId)
        {
            CartItem cartItem = new CartItem()
            {
                User = await _usersService.GetById(userId),
                Product = await _productService.GetById(productId),
                Quantity = 1
            };

            await _dbContext.CartItem.AddAsync(cartItem);
            await _dbContext.SaveChangesAsync();
        }

        public async void RemoveFromCart(Guid userId, Guid productId)
        {
            await _dbContext.CartItem
                .Where(c => c.User.UserId == userId)
                .Where(c => c.Product.ProductId == productId)
                .ExecuteDeleteAsync();
        }

        public async Task<Guid> OrderCart(Guid userId, Guid deliveryMethodId, Guid paymentMethodId)
        {
            Guid paidOrderId = await _paidOrderService.Create(userId, deliveryMethodId, paymentMethodId);

            foreach (var i in await this.GetAll(userId))
            {
                await _dbContext.PaidOrderItem.AddAsync(
                    _mapper.Map<PaidOrderItem>(i));

                await _dbContext.CartItem.Where(ci => ci == i).ExecuteDeleteAsync();
            }
        }

    }
}
