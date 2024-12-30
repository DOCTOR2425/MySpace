namespace InstrumentStore.Domain.Contracts.User
{
    public record UserOrderInfo(
        Guid UserId,
        string FirstName,
        string Telephone,
        string EMail,
        string City,
        string Street,
        string HouseNumber,
        string Entrance,
        string Flat
    );
}
