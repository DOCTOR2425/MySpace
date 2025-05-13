namespace InstrumentStore.Domain.Contracts.Products
{
	public class UserProductStats
	{
		public int CartCount { get; set; }
		public bool IsInComparison { get; set; }

		public UserProductStats(int cartCount, bool isInComparison)
		{
			CartCount = cartCount;
			IsInComparison = isInComparison;
		}

		public UserProductStats() { }
	}
}
