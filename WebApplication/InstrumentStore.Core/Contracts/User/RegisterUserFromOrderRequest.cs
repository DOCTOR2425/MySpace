namespace InstrumentStore.Domain.Contracts.User
{
    public record RegisterUserFromOrderRequest(
        string FirstName,
        string Surname,
        string Telephone,
        string Email,

        string City,
        string Street,
        string HouseNumber,
        string Entrance,
        string Flat);
}
