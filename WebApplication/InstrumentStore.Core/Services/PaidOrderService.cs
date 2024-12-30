using InstrumentStore.Domain.DataBase.Models;
using InstrumentStore.Domain.DataBase;
using Microsoft.EntityFrameworkCore;
using InstrumentStore.Domain.Abstractions;

namespace InstrumentStore.Domain.Services
{
    public class PaidOrderService : IPaidOrderService
    {
        private readonly InstrumentStoreDBContext _dbContext;
        private readonly IUsersService _usersService;
        private readonly IDeliveryMethodService _deliveryMethodService;
        private readonly IPaymentMethodService _paymentMethodService;

        public PaidOrderService(InstrumentStoreDBContext dbContext,
            IUsersService usersService,
            IDeliveryMethodService deliveryMethodService,
            IPaymentMethodService paymentMethodService)
        {
            _dbContext = dbContext;
            _usersService = usersService;
            _deliveryMethodService = deliveryMethodService;
            _paymentMethodService = paymentMethodService;
        }

        public async Task<List<PaidOrder>> GetAll(Guid userId)
        {
            return await _dbContext.PaidOrder
                .Include(o => o.User)
                .Include(o => o.PaymentMethod)
                .Include(o => o.DeliveryMethod)
                .Where(c => c.User.UserId == userId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<PaidOrder> GetById(Guid orderId)
        {
            return await _dbContext.PaidOrder
                .Include(o => o.User)
                .Include(o => o.PaymentMethod)
                .Include(o => o.DeliveryMethod)
                .Where(c => c.PaidOrderId == orderId)
                .FirstOrDefaultAsync();
        }

        public async Task<List<PaidOrderItem>> GetAllItemsByOrder(Guid paidOrderId)
        {
            List<PaidOrderItem> itemsInOrder = new List<PaidOrderItem>();

            foreach(var i in await _dbContext.PaidOrderItem.Include(i=>i.PaidOrder).ToListAsync())
                if(i.PaidOrder.PaidOrderId == paidOrderId)
                    itemsInOrder.Add(i);

            return itemsInOrder;
        }

        public async Task<Guid> Create(Guid userId, Guid deliveryMethodId, Guid paymentMethodId)
        {
            PaidOrder paidOrder = new PaidOrder()
            {
                PaidOrderId = Guid.NewGuid(),
                PaymentDate = DateTime.Now,
                User = await _usersService.GetById(userId),
                DeliveryMethod = await _deliveryMethodService.GetById(deliveryMethodId),
                PaymentMethod = await _paymentMethodService.GetById(paymentMethodId),
            };

            await _dbContext.PaidOrder.AddAsync(paidOrder);
            await _dbContext.SaveChangesAsync();

            return paidOrder.PaidOrderId;
        }
    }
}
