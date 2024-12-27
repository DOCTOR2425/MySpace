namespace InstrumentStore.Domain.Contracts.User
{
    public record LoginUserRequest(
        string EMail,
        string Password);
}
