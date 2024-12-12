namespace InstrumentStore.Domain.Contracts.Users
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
        string Entrance,
        string Flat);
}
