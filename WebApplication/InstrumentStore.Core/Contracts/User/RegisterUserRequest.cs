namespace InstrumentStore.Domain.Contracts.User
{
    public record RegisterUserRequest(
        string FirstName,
        string Surname,
        string Telephone,
        string Email,
        string Password,

        string City,
        string Street,
        string HouseNumber,
        string Entrance,
        string Flat);
}
