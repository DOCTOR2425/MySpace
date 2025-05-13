using AutoMapper;
using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.DataBase;
using InstrumentStore.Domain.DataBase.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Linq;
using Excel = Microsoft.Office.Interop.Excel;

namespace InstrumentStore.Domain.Services
{
	public class ReportService : IReportService
	{
		private readonly InstrumentStoreDBContext _dbContext;
		private readonly IUserService _usersService;
		private readonly IProductService _productService;
		private readonly IProductCategoryService _productCategoryService;
		private readonly IPaidOrderService _paidOrderService;
		private readonly IDeliveryMethodService _deliveryMethodService;
		private readonly IPaymentMethodService _paymentMethodService;
		private readonly IEmailService _emailService;
		private readonly IJwtProvider _jwtProvider;
		private readonly IMapper _mapper;
		private readonly IConfiguration _config;

		public string NewExcelFileName
		{
			get { return Environment.CurrentDirectory + "\\wwwroot\\newFile.xlsx"; }
		}

		public ReportService(InstrumentStoreDBContext dbContext,
			IUserService usersService,
			IProductService productService,
			IPaidOrderService paidOrderService,
			IDeliveryMethodService eliveryMethodService,
			IPaymentMethodService paymentMethodService,
			IMapper mapper,
			IEmailService emailService,
			IConfiguration config,
			IJwtProvider jwtProvider,
			IProductCategoryService productCategoryService)
		{
			_dbContext = dbContext;
			_usersService = usersService;
			_productService = productService;
			_paidOrderService = paidOrderService;
			_deliveryMethodService = eliveryMethodService;
			_paymentMethodService = paymentMethodService;
			_mapper = mapper;
			_emailService = emailService;
			_config = config;
			_jwtProvider = jwtProvider;
			_productCategoryService = productCategoryService;
		}

		private IQueryable<PaidOrder> GetOrdersInTimeSpan(DateTime from, DateTime to)
		{
			return _dbContext.PaidOrder
				.AsQueryable()
				.Where(o => o.OrderDate >= from && o.OrderDate <= to);
		}

		private async Task<List<PaidOrderItem>> ItemsFromOrders(List<PaidOrder> paidOrders)
		{
			List<PaidOrderItem> paidOrderItems = new List<PaidOrderItem>();
			foreach (var order in paidOrders)
			{
				paidOrderItems.AddRange(await _paidOrderService
					.GetAllItemsByOrder(order.PaidOrderId));
			}

			return paidOrderItems;
		}

		public async Task<string> GenerateSalesByCategoryOverTimeReport(DateTime from, DateTime to)
		{
			Dictionary<string, decimal> categoriesStats = await _dbContext.PaidOrderItem
				.Include(i => i.Product.ProductCategory)
				.Include(i => i.PaidOrder)
				.Where(i => i.PaidOrder.ReceiptDate != PaidOrderService.OrderPendingStatus &&
					i.PaidOrder.ReceiptDate != PaidOrderService.OrderCanceledStatus)
				.GroupBy(i => i.Product.ProductCategory.Name)
				.Select(g => new
				{
					CategoryName = g.Key,
					TotalSales = g.Sum(i => i.Price * i.Quantity)
				})
				.OrderByDescending(x => x.TotalSales)
				.ToDictionaryAsync(
					x => x.CategoryName,
					x => x.TotalSales
				);

			await GenerateExcelReport(categoriesStats,
				"Продажи по категориям",
				"Категории",
				"Заработок (BYN)",
				"ЗАРАБОТОК ПО КАТЕГОРИЯМ ЗА " + ((to - from).Days / 30) + " МЕСЯЦЕВ");

			return NewExcelFileName;
		}

		private async Task<string> GenerateExcelReport(
			Dictionary<string, decimal> sales,
			string reportName,
			string columnsName = "",
			string rowsName = "",
			string chartTitle = "")
		{
			var excelApp = new Excel.Application();
			var workbook = excelApp.Workbooks.Add();

			var salesChart = await CreateNewChart(workbook, sales, reportName);

			salesChart.Axes(Excel.XlAxisType.xlCategory).AxisTitle.Characters.Text = columnsName;
			salesChart.Axes(Excel.XlAxisType.xlValue).AxisTitle.Characters.Text = rowsName;
			salesChart.ChartTitle.Characters.Text = chartTitle;

			File.Delete(NewExcelFileName);
			workbook.SaveAs(NewExcelFileName);
			workbook.Close(SaveChanges: false);
			excelApp.Quit();

			return NewExcelFileName;
		}
		private async Task<Excel.Chart> CreateNewChart(
			Excel.Workbook currentApp,
			Dictionary<string, decimal> data,
			string chartName)
		{
			var excelApp = (Excel.Worksheet)currentApp.Sheets[1];
			excelApp.Name = "Для " + chartName;

			for (int i = 0; i < data.Count; i++)
			{
				excelApp.Cells[i + 1, 1] = data.Keys.ToArray()[i];
				excelApp.Cells[i + 1, 2] = data.Values.ToArray()[i];
			}

			currentApp.Charts.Add();
			currentApp.ActiveChart.ChartType = Excel.XlChartType.xlColumnClustered;
			currentApp.ActiveChart.HasLegend = false;
			currentApp.ActiveChart.HasTitle = true;

			currentApp.ActiveChart.Axes(Excel.XlAxisType.xlCategory).HasTitle = true;
			currentApp.ActiveChart.Axes(Excel.XlAxisType.xlValue).HasTitle = true;
			currentApp.ActiveChart.Name = chartName;

			return currentApp.ActiveChart;
		}

		private async Task<Dictionary<Product, int>> GetProductsStats(DateTime from, DateTime to)
		{
			List<PaidOrderItem> itemsOverTime = await _dbContext.PaidOrderItem
				.AsQueryable()
				.Include(i => i.Product)
				.Include(i => i.Product.ProductCategory)
				.Where(i => i.PaidOrder.OrderDate >= from && i.PaidOrder.OrderDate <= to)
				.ToListAsync();

			Dictionary<Product, int> productsWithSalesCount = new Dictionary<Product, int>();

			foreach (PaidOrderItem item in itemsOverTime)
			{
				try
				{
					productsWithSalesCount.Add(item.Product, item.Quantity);
				}
				catch
				{
					productsWithSalesCount[item.Product] += item.Quantity;
				}
			}

			return productsWithSalesCount;
		}

		public async Task<string> GenerateStockOverTimeReport(DateTime from, DateTime to)
		{
			var excelApp = new Excel.Application();
			var workbook = excelApp.Workbooks.Add();
			var table = (Excel.Worksheet)workbook.Sheets[1];
			table.Name = "Склад";
			table.Cells[1, 1] = "Категория";
			table.Cells[1, 2] = "Товар";
			table.Cells[1, 3] = "Остаток на складе";
			table.Cells[1, 4] = "Проданно за отчётный период";

			List<ProductCategory> categories = await _productCategoryService.GetAll();
			Dictionary<Product, int> productsWithSalesCount = await GetProductsStats(from, to);

			int currentRow = 1;

			Dictionary<Product, int> productsByCategory = new Dictionary<Product, int>();
			foreach (var category in categories)
			{
				currentRow++;
				table.Cells[currentRow, 1] = category.Name;

				productsByCategory = productsWithSalesCount
					.Where(it => it.Key.ProductCategory.ProductCategoryId == category.ProductCategoryId)
					.OrderBy(it => it.Value)
					.ToDictionary();

				foreach (var productWithSalesCount in productsByCategory)
				{
					currentRow++;
					table.Cells[currentRow, 2] = productWithSalesCount.Key.Name;
					table.Cells[currentRow, 3] = productWithSalesCount.Key.Quantity;
					table.Cells[currentRow, 4] = productWithSalesCount.Value;
				}
			}

			File.Delete(NewExcelFileName);
			workbook.SaveAs(NewExcelFileName);
			workbook.Close(SaveChanges: false);
			excelApp.Quit();

			return NewExcelFileName;
		}

		public async Task<string> GenerateOrdersReport(DateTime from, DateTime to)
		{
			var excelApp = new Excel.Application();
			var workbook = excelApp.Workbooks.Add();
			var table = (Excel.Worksheet)workbook.Sheets[1];
			table.Name = "Информация по заказам";
			table.Cells[1, 1] = "Дата";
			table.Cells[1, 2] = "Всего\n\rзаказов";
			table.Cells[1, 3] = "Доставлено";
			table.Cells[1, 4] = "Отменено";
			table.Cells[1, 5] = "В обработке";
			table.Cells[1, 6] = "Общий оборот\n\r(BYN)";
			table.Cells[1, 7] = "Средний чек\n\r(BYN)";

			List<PaidOrder> paidOrders = await GetOrdersInTimeSpan(from, to)
				.OrderBy(o => o.OrderDate)
				.ToListAsync();
			decimal amount = 0;
			decimal mounthAverageOrderReceipt = 0;
			List<PaidOrder> monthsOrders = null;

			int currentRow = 2;
			for (DateTime date = paidOrders[0].OrderDate; date <= to; date = date.AddMonths(1))
			{
				monthsOrders = paidOrders
						.Where(o => o.OrderDate.Month == date.Month &&
							o.OrderDate.Year == date.Year)
						.ToList();
				(amount, mounthAverageOrderReceipt) =
					await CalculateMounthAverageOrderReceipt(
						monthsOrders);

				table.Cells[currentRow, 1] = date.ToString("yyyy MMMM");
				table.Cells[currentRow, 2] = monthsOrders.Count;

				table.Cells[currentRow, 3] = monthsOrders
					.Where(o => o.ReceiptDate != PaidOrderService.OrderCanceledStatus &&
						o.ReceiptDate != PaidOrderService.OrderPendingStatus)
					.Count();
				table.Cells[currentRow, 4] = monthsOrders
					.Where(o => o.ReceiptDate == PaidOrderService.OrderCanceledStatus)
					.Count();
				table.Cells[currentRow, 5] = monthsOrders
					.Where(o => o.ReceiptDate == PaidOrderService.OrderPendingStatus)
					.Count();

				table.Cells[currentRow, 6] = amount;
				table.Cells[currentRow, 7] = mounthAverageOrderReceipt;

				currentRow++;
			}

			table.Cells[currentRow, 1] = "Итого";
			table.Cells[currentRow, 2] = paidOrders.Count;

			table.Cells[currentRow, 3] = paidOrders
					.Where(o => o.ReceiptDate != PaidOrderService.OrderCanceledStatus &&
						o.ReceiptDate != PaidOrderService.OrderPendingStatus)
					.Count();
			table.Cells[currentRow, 4] = paidOrders
				.Where(o => o.ReceiptDate == PaidOrderService.OrderCanceledStatus)
				.Count();
			table.Cells[currentRow, 5] = paidOrders
				.Where(o => o.ReceiptDate == PaidOrderService.OrderPendingStatus)
				.Count();

			List<PaidOrderItem> items = await ItemsFromOrders(paidOrders
				.Where(_paidOrderService.IsOrderInProcessing)
				.ToList());
			table.Cells[currentRow, 6] = items.Sum(i => i.Quantity * i.Price);
			table.Cells[currentRow, 7] = items
				.GroupBy(item => item.PaidOrder.PaidOrderId)
				.Select(group => group.Sum(item => item.Quantity * item.Price))
				.Average();

			File.Delete(NewExcelFileName);
			workbook.SaveAs(NewExcelFileName);
			workbook.Close(SaveChanges: false);
			excelApp.Quit();

			return NewExcelFileName;
		}

		private async Task<(decimal, decimal)> CalculateMounthAverageOrderReceipt(
			List<PaidOrder> paidOrders)
		{
			if (paidOrders.Any() == false)
				return (0, 0);

			List<PaidOrderItem> paidOrderItems = await ItemsFromOrders(paidOrders
				.Where(_paidOrderService.IsOrderInProcessing)
				.ToList());

			decimal amount = paidOrderItems.Sum(i => i.Quantity * i.Price);
			decimal mounthAverageOrderReceipt = paidOrderItems
				.GroupBy(item => item.PaidOrder.PaidOrderId)
				.Select(group => group.Sum(item => item.Quantity * item.Price))
				.Average();

			return (amount, mounthAverageOrderReceipt);
		}

		public async Task<string> GenerateProfitFromUsersReport(DateTime from, DateTime to)
		{
			var excelApp = new Excel.Application();
			var workbook = excelApp.Workbooks.Add();
			var table = (Excel.Worksheet)workbook.Sheets[1];
			table.Name = "Доход от покупателей";
			table.Cells[1, 1] = "Покупатель";
			table.Cells[1, 2] = "Количество заказов";
			table.Cells[1, 3] = "Общий доход (BYN)";
			table.Cells[1, 4] = "Средний чек (BYN)";

			List<PaidOrder> paidOrders = await GetOrdersInTimeSpan(from, to)
				.Where(o => o.ReceiptDate != PaidOrderService.OrderCanceledStatus &&
					o.ReceiptDate != PaidOrderService.OrderPendingStatus)
				.Include(o => o.User)
				.ToListAsync();

			int usersOrderCount = 0;
			decimal usersAmountProfit = 0;
			decimal usersAverageProfit = 0;

			int currentRow = 2;
			foreach (User user in paidOrders.Select(o => o.User).Distinct())
			{
				(usersOrderCount, usersAmountProfit, usersAverageProfit) =
					await CalculateUserOrdersStats(user, paidOrders);

				table.Cells[currentRow, 1] = $"{user.Surname} {user.FirstName}";
				table.Cells[currentRow, 2] = usersOrderCount;
				table.Cells[currentRow, 3] = usersAmountProfit;
				table.Cells[currentRow, 4] = usersAverageProfit;

				currentRow++;
			}

			File.Delete(NewExcelFileName);
			workbook.SaveAs(NewExcelFileName);

			workbook.Close(SaveChanges: false);
			excelApp.Quit();

			return NewExcelFileName;
		}

		private async Task<(int, decimal, decimal)> CalculateUserOrdersStats(
			User user,
			List<PaidOrder> paidOrders)
		{
			if (user == null || paidOrders.Any() == false)
				return (0, 0, 0);

			List<PaidOrder> userOrders = paidOrders
				.Where(o => o.User == user)
				.ToList();
			int usersOrderCount = userOrders.Count;

			List<PaidOrderItem> paidOrderItems = await ItemsFromOrders(userOrders);

			decimal usersAmountProfit = paidOrderItems.Sum(i => i.Quantity * i.Price);
			decimal usersAverageProfit = paidOrderItems
				.GroupBy(item => item.PaidOrder.PaidOrderId)
				.Select(group => group.Sum(item => item.Quantity * item.Price))
				.Average();

			return (usersOrderCount, usersAmountProfit, usersAverageProfit);
		}
	}
}
