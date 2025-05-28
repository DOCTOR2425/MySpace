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
		private readonly IUserService _usersService;
		private readonly IDeliveryMethodService _deliveryMethodService;
		private readonly IPromoCodeService _promoCodeService;

		private int PageSize { get; } = 5;
		public static DateTime OrderPendingStatus { get; } = DateTime.MinValue;
		public static DateTime OrderCanceledStatus { get; } = DateTime.MaxValue;

		public PaidOrderService(InstrumentStoreDBContext dbContext,
			IUserService usersService,
			IDeliveryMethodService deliveryMethodService,
			IPromoCodeService promoCodeService)
		{
			_dbContext = dbContext;
			_usersService = usersService;
			_deliveryMethodService = deliveryMethodService;
			_promoCodeService = promoCodeService;
		}

		public async Task<List<PaidOrder>> GetAllByUserId(Guid userId)
		{
			return await _dbContext.PaidOrder
				.Include(o => o.User)
				.Include(o => o.DeliveryMethod)
				.Include(o => o.PromoCode)
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
				.Include(o => o.PromoCode)
				.FirstOrDefaultAsync(c => c.PaidOrderId == orderId);
		}

		public async Task<List<PaidOrderItem>> GetAllItemsByOrder(Guid paidOrderId)
		{
			return await _dbContext.PaidOrderItem
				.AsQueryable()
				.Include(i => i.PaidOrder)
				.Include(i => i.Product)
				.Include(i => i.Product.ProductCategory)
				.Include(i => i.Product.Brand)
				.Include(i => i.Product.Country)
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
				PromoCode = await _promoCodeService
					.GetIfActive(orderCartRequest.PromoCode)
			};

			if (await _deliveryMethodService.IsHomeDelivery(orderCartRequest.DeliveryMethodId)
				&& orderCartRequest.UserDelivaryAddress != null)
			{
				DeliveryAddress deliveryAddress = new DeliveryAddress()
				{
					DeliveryAddressId = Guid.NewGuid(),
					PaidOrder = paidOrder,
					City = orderCartRequest.UserDelivaryAddress.City,
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
			(await GetById(orderId)).ReceiptDate = OrderCanceledStatus;

			foreach (var item in await GetAllItemsByOrder(orderId))
				item.Product.Quantity += item.Quantity;

			await _dbContext.SaveChangesAsync();

			return orderId;
		}

		public async Task<List<PaidOrder>> GetProcessingOrders()
		{
			return await _dbContext.PaidOrder
				.Include(o => o.User)
				.Include(o => o.DeliveryMethod)
				.Include(o => o.PromoCode)
				.Where(o => o.ReceiptDate == OrderPendingStatus)
				.OrderByDescending(o => o.OrderDate)
				.ToListAsync();
		}

		public async Task<DeliveryAddress?> GetDeliveryAddressByOrderId(Guid orderId)
		{
			return await _dbContext.DeliveryAddress
				.FirstOrDefaultAsync(a => a.PaidOrder.PaidOrderId == orderId);
		}

		public async Task<List<PaidOrder>> GetAll(int page)
		{
			return await _dbContext.PaidOrder
				.OrderByDescending(o => o.OrderDate)
				.Skip((page - 1) * PageSize)
				.Take(PageSize)
				.Include(o => o.User)
				.Include(o => o.DeliveryMethod)
				.Include(o => o.PromoCode)
				.ToListAsync();
		}

		public async Task<DeliveryAddress?> GetLastAddressByUser(Guid userId)
		{
			List<PaidOrder> paidOrders = await GetAllByUserId(userId);

			DeliveryAddress? address = null;
			foreach (var order in paidOrders)
			{
				address = _dbContext.DeliveryAddress
					.FirstOrDefault(a => a.PaidOrder == order);
				if (address != null)
					break;
			}

			return address;
		}

		public bool IsOrderInProcessing(PaidOrder paidOrder)
		{
			if (paidOrder.ReceiptDate != OrderCanceledStatus &&
						paidOrder.ReceiptDate != OrderPendingStatus)
				return true;
			return false;
		}
	}
}
