namespace InstrumentStore.Domain.Contracts.Users
{
    public record LoginUserRequest(
        string EMail,
        string Password);
}
