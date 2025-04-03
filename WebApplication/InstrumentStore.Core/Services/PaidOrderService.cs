using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.Contracts.Cart;
using InstrumentStore.Domain.DataBase;
using InstrumentStore.Domain.DataBase.Models;
using Microsoft.EntityFrameworkCore;

namespace InstrumentStore.Domain.Services
{
    public class PaidOrderService : IPaidOrderService
    {
        private readonly InstrumentStoreDBContext _dbContext;
        private readonly IUsersService _usersService;
        private readonly ICityService _cityService;
        private readonly IDeliveryMethodService _deliveryMethodService;

        private const int PageSize = 5;

        public PaidOrderService(InstrumentStoreDBContext dbContext,
            IUsersService usersService,
            IDeliveryMethodService deliveryMethodService,
            ICityService cityService)
        {
            _dbContext = dbContext;
            _usersService = usersService;
            _deliveryMethodService = deliveryMethodService;
            _cityService = cityService;
        }

        public async Task<List<PaidOrder>> GetAllByUserId(Guid userId)
        {
            return await _dbContext.PaidOrder
                .Include(o => o.User)
                .Include(o => o.DeliveryMethod)
                .Where(c => c.User.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<PaidOrder> GetById(Guid orderId)
        {
            return await _dbContext.PaidOrder
                .Include(o => o.User)
                .Include(o => o.DeliveryMethod)
                .Where(c => c.PaidOrderId == orderId)
                .FirstOrDefaultAsync();
        }

        public async Task<List<PaidOrderItem>> GetAllItemsByOrder(Guid paidOrderId)
        {
            return await _dbContext.PaidOrderItem
                .Include(i => i.PaidOrder)
                .Include(i => i.Product)
                .Where(i => i.PaidOrder.PaidOrderId == paidOrderId)
                .ToListAsync();
        }

        public async Task<Guid> Create(Guid userId, OrderRequest orderCartRequest)
        {
            PaidOrder paidOrder = new PaidOrder()
            {
                PaidOrderId = Guid.NewGuid(),
                OrderDate = DateTime.Now,
                User = await _usersService.GetById(userId),
                DeliveryMethod = await _deliveryMethodService
                    .GetById(orderCartRequest.DeliveryMethodId),
                PaymentMethod = orderCartRequest.PaymentMethod,
            };

            if (await _deliveryMethodService.IsHomeDelivery(orderCartRequest.DeliveryMethodId)
                && orderCartRequest.UserDelivaryAddress != null)
            {
                DeliveryAddress deliveryAddress = new DeliveryAddress()
                {
                    DeliveryAddressId = Guid.NewGuid(),
                    PaidOrder = paidOrder,
                    City = await _cityService.GetByName(orderCartRequest.UserDelivaryAddress.City),
                    Street = orderCartRequest.UserDelivaryAddress.Street,
                    HouseNumber = orderCartRequest.UserDelivaryAddress.HouseNumber,
                    Entrance = orderCartRequest.UserDelivaryAddress.Entrance,
                    Flat = orderCartRequest.UserDelivaryAddress.Flat
                };

                await _dbContext.DeliveryAddress.AddAsync(deliveryAddress);
            }

            await _dbContext.PaidOrder.AddAsync(paidOrder);
            await _dbContext.SaveChangesAsync();

            return paidOrder.PaidOrderId;
        }

        public async Task<Guid> CloseOrder(Guid orderId)
        {
            (await GetById(orderId)).ReceiptDate = DateTime.Now;
            await _dbContext.SaveChangesAsync();

            return orderId;
        }

        public async Task<Guid> CancelOrder(Guid orderId)
        {
            (await GetById(orderId)).ReceiptDate = DateTime.MaxValue;
            await _dbContext.SaveChangesAsync();

            return orderId;
        }

        public async Task<List<PaidOrder>> GetProcessingOrders()
        {
            return await _dbContext.PaidOrder
                .Include(o => o.User)
                .Include(o => o.User.UserRegistrInfo)
                .Include(o => o.DeliveryMethod)
                .Where(o => o.ReceiptDate == DateTime.MinValue)
                .ToListAsync();
        }

        public async Task<DeliveryAddress?> GetDeliveryAddressByOrderId(Guid orderId)
        {
            return await _dbContext.DeliveryAddress
                .Include(a => a.City)
                .FirstOrDefaultAsync(a => a.PaidOrder.PaidOrderId == orderId);
        }

        public async Task<List<PaidOrder>> GetAll(int page)
        {
            return await _dbContext.PaidOrder
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .Include(o => o.User)
                .Include(o => o.DeliveryMethod)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }
    }
}
