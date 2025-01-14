namespace InstrumentStore.Domain.Contracts.User
{
    public record RegisterUserFromOrderRequest(
        string FirstName,
        string Surname,
        string Telephone,
        string EMail,

        string City,
        string Street,
        string HouseNumber,
        string Entrance,
        string Flat);
}
