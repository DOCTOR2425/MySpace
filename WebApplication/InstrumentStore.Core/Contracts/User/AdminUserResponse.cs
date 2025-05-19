namespace InstrumentStore.Domain.Contracts.User
{
	public class AdminUserResponse
	{
		public Guid UserId { get; set; }
		public string FirstName { get; set; } = string.Empty;
		public string Surname { get; set; } = string.Empty;
		public string Telephone { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;

		public int OrderCount { get; set; }

		public DateTime RegistrationDate { get; set; } = DateTime.Now;
		public DateTime? BlockDate { get; set; } = DateTime.Now;
		public string? BlockDetails { get; set; } = string.Empty;
	}
}
