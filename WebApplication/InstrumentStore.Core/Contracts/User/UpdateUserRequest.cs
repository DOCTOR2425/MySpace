namespace InstrumentStore.Domain.Contracts.User
{
	public class UpdateUserRequest
	{
		public string FirstName { get; set; } = string.Empty;
		public string Surname { get; set; } = string.Empty;
		public string Telephone { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
	}
}
