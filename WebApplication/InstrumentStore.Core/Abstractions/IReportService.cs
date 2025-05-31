namespace InstrumentStore.Domain.Abstractions
{
	public interface IReportService
	{
		Task<string> GenerateSalesByCategoryOverTimeReport(DateTime from, DateTime to);
		Task<string> GenerateStockOverTimeReport(DateTime from, DateTime to);
		Task<string> GenerateOrdersReport(DateTime from, DateTime to);
		Task<string> GenerateProfitFromUsersReport(DateTime from, DateTime to);
		Task<string> GeneratePopylarProductsBySeasonsReport(DateTime from, DateTime to);
	}
}