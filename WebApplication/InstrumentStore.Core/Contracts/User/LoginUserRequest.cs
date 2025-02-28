namespace InstrumentStore.Domain.Contracts.User
{
	public record LoginUserRequest(
		string Email,
		string Password);
}
