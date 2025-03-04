using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.DataBase;
using InstrumentStore.Domain.DataBase.Models;
using Microsoft.EntityFrameworkCore;

namespace InstrumentStore.Domain.Services
{
	public class PaidOrderService : IPaidOrderService
	{
		private readonly InstrumentStoreDBContext _dbContext;
		private readonly IUsersService _usersService;
		private readonly IDeliveryMethodService _deliveryMethodService;

		public PaidOrderService(InstrumentStoreDBContext dbContext,
			IUsersService usersService,
			IDeliveryMethodService deliveryMethodService)
		{
			_dbContext = dbContext;
			_usersService = usersService;
			_deliveryMethodService = deliveryMethodService;
		}

		public async Task<List<PaidOrder>> GetAll(Guid userId)
		{
			return await _dbContext.PaidOrder
				.Include(o => o.User)
				.Include(o => o.DeliveryMethod)
				.Where(c => c.User.UserId == userId)
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
			List<PaidOrderItem> itemsInOrder = new List<PaidOrderItem>();

			foreach (var i in await _dbContext.PaidOrderItem.Include(i => i.PaidOrder).ToListAsync())
				if (i.PaidOrder.PaidOrderId == paidOrderId)
					itemsInOrder.Add(i);

			return itemsInOrder;
		}

		public async Task<Guid> Create(Guid userId, Guid deliveryMethodId, string paymentMethod)
		{
			PaidOrder paidOrder = new PaidOrder()
			{
				PaidOrderId = Guid.NewGuid(),
				OrderDate = DateTime.Now,
				User = await _usersService.GetById(userId),
				DeliveryMethod = await _deliveryMethodService.GetById(deliveryMethodId),
				PaymentMethod = paymentMethod,
			};

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
	}
}
