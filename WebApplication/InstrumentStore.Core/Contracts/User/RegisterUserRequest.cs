namespace InstrumentStore.Domain.Contracts.User
{
    public record RegisterUserRequest(
        string FirstName,
        string Surname,
        string Patronymic,
        string Telephone,
        string EMail,
        string Password,

        string City,
        string Street,
        string HouseNumber,
        string Entrance,
        string Flat);
}
