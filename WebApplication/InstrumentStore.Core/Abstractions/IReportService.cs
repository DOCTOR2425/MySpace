namespace InstrumentStore.Domain.Abstractions
{
	public interface IReportService
	{
		Task<string> GenerateReportSalesByCategoryOverTime(DateTime from, DateTime to);
		Task<string> GenerateWordReport(DateTime from, DateTime to);
	}
}