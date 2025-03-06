namespace InstrumentStore.Domain.Contracts.User
{
	public class UserDeliveryAddress
	{
		public string City { get; set; } = string.Empty;
		public string Street { get; set; } = string.Empty;
		public string HouseNumber { get; set; } = string.Empty;
		public string Entrance { get; set; } = string.Empty;
		public string Flat { get; set; } = string.Empty;
	}
}
