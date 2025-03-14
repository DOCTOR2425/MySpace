using AutoMapper;
using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.DataBase;
using InstrumentStore.Domain.DataBase.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Office.Interop.Excel;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Excel = Microsoft.Office.Interop.Excel;
using Word = Microsoft.Office.Interop.Word;

namespace InstrumentStore.Domain.Services
{
    public class ReportService : IReportService
    {
        private readonly InstrumentStoreDBContext _dbContext;
        private readonly IUsersService _usersService;
        private readonly IProductService _productService;
        private readonly IProductCategoryService _productCategoryService;
        private readonly IPaidOrderService _paidOrderService;
        private readonly IDeliveryMethodService _deliveryMethodService;
        private readonly IPaymentMethodService _paymentMethodService;
        private readonly IEmailService _emailService;
        private readonly IJwtProvider _jwtProvider;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

        public string WordTeplateName
        {
            get { return Environment.CurrentDirectory + "\\wwwroot\\Template.docx"; }
        }
        public string NewWordFileName
        {
            get { return Environment.CurrentDirectory + "\\wwwroot\\newFile.docx"; }
        }
        public string NewExcelFileName
        {
            get { return Environment.CurrentDirectory + "\\wwwroot\\newFile.xlsx"; }
        }

        public ReportService(InstrumentStoreDBContext dbContext,
            IUsersService usersService,
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

        public async Task<string> GenerateWordReport(DateTime from, DateTime to)
        {
            await GenerateWordReport(new Dictionary<string, string>()
            {
                { "<reportName>", "GenerateReportSalesByCategoryOverTime" },
                { "<body>", $"{from}\n{to}" }
            });

            return NewWordFileName;
        }

        private async Task<string> GenerateWordReport(Dictionary<string, string> pairsToChange)
        {
            if (File.Exists(WordTeplateName) == false)
            {
                throw new FieldAccessException("Не удалось найти шаблон файла для составления договора");
            }

            var wordApp = new Word.Application();
            Word.Document doc = null;

            try
            {
                File.Copy(WordTeplateName, NewWordFileName, overwrite: true);

                doc = wordApp.Documents.Open(NewWordFileName);
                object mis = Type.Missing;

                Word.Find find = wordApp.Selection.Find;

                for (int i = 0; i < pairsToChange.Count; i++)
                {
                    find.Text = pairsToChange.ElementAt(i).Key;
                    find.Replacement.Text = pairsToChange.ElementAt(i).Value;

                    object wrap = Word.WdFindWrap.wdFindContinue;
                    object replace = Word.WdReplace.wdReplaceAll;

                    find.Execute(FindText: mis, MatchSoundsLike: mis, Forward: true,
                        Wrap: wrap, ReplaceWith: mis, Replace: replace);
                }

                doc.Save();
            }
            catch (COMException ex)
            {
                Console.WriteLine("Ошибка в Word");
                throw ex;
            }
            catch (IOException ex)
            {
                throw new Exception(ex.Message + "\nОшибка файла");
            }
            finally
            {
                if (doc != null)
                {
                    doc.Close(SaveChanges: false);
                    Marshal.ReleaseComObject(doc);
                }

                if (wordApp != null)
                {
                    wordApp.Quit();
                    Marshal.ReleaseComObject(wordApp);
                }
            }

            return NewWordFileName;
        }

        public async Task<string> GenerateReportSalesByCategoryOverTime(DateTime from, DateTime to)
        {
            Dictionary<string, decimal> categoriesStats = new Dictionary<string, decimal>();
            List<PaidOrderItem> paidOrderItems = await _dbContext.PaidOrderItem
                .AsQueryable()
                .Include(i => i.Product)
                .Include(i => i.Product.ProductCategory)
                .Where(i => i.PaidOrder.OrderDate >= from && i.PaidOrder.OrderDate <= to)
                .ToListAsync();

            foreach (ProductCategory category in _dbContext.ProductCategory)
            {
                categoriesStats.Add(category.Name,
                    paidOrderItems.Where(
                        p => p.Product.ProductCategory.ProductCategoryId == category.ProductCategoryId)
                    .Sum(i => i.Price * i.Quantity));
            }

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

            foreach (var item in itemsOverTime)
            {
                productsWithSalesCount.Add(item.Product, itemsOverTime
                    .Where(i => i.Product.ProductId == item.Product.ProductId)
                    .Sum(i => i.Quantity));
            }

            return productsWithSalesCount;
        }

        public async Task<string> GenerateStockReportOverTime(DateTime from, DateTime to)
        {
            var excelApp = new Excel.Application();
            var workbook = excelApp.Workbooks.Add();
            var table = (Excel.Worksheet)workbook.Sheets[1];
            table.Name = "Склад";

            List<ProductCategory> categories = await _productCategoryService.GetAll();
            Dictionary<Product, int> productsWithSalesCount = await GetProductsStats(from, to);

            table.Cells[1, 1] = "Категория";
            table.Cells[1, 2] = "Товар";
            table.Cells[1, 3] = "Остаток на складе";
            table.Cells[1, 4] = "Проданно за отчётный период";

            int currentRow = 1;

            Dictionary<Product, int> productsByCategory = new Dictionary<Product, int>();
            foreach(var category in categories)
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
    }
}
