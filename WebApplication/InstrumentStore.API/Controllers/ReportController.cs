using InstrumentStore.Domain.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InstrumentStore.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ReportController : ControllerBase
	{
		private readonly IReportService _reportService;

		public ReportController(IReportService reportService)
		{
			_reportService = reportService;
		}

		[HttpGet("generate-report-sales-by-category-over-time")]
		public async Task<ActionResult> GenerateSalesByCategoryOverTimeReport(
			[FromQuery] DateTime from,
			[FromQuery] DateTime to)
		{
			string filePath = await _reportService.GenerateSalesByCategoryOverTimeReport(from, to);
			var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
			string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

			return File(fileBytes, mimeType, Path.GetFileName(filePath));
		}

		[HttpGet("generate-stock-report-over-time")]
		public async Task<ActionResult> GenerateStockOverTimeReport(
			[FromQuery] DateTime from,
			[FromQuery] DateTime to)
		{
			string filePath = await _reportService.GenerateStockOverTimeReport(from, to);
			var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
			string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

			return File(fileBytes, mimeType, Path.GetFileName(filePath));
		}

		[HttpGet("generate-orders-report")]
		public async Task<ActionResult> GenerateOrdersReport(
			[FromQuery] DateTime from,
			[FromQuery] DateTime to)
		{
			string filePath = await _reportService.GenerateOrdersReport(from, to);
			var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
			string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

			return File(fileBytes, mimeType, Path.GetFileName(filePath));
		}

		[HttpGet("generate-profit-from-users-report")]
		public async Task<ActionResult> GenerateProfitFromUsersReport(
			[FromQuery] DateTime from,
			[FromQuery] DateTime to)
		{
			string filePath = await _reportService.GenerateProfitFromUsersReport(from, to);
			var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
			string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

			return File(fileBytes, mimeType, Path.GetFileName(filePath));
		}
	}
}
