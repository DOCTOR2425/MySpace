namespace InstrumentStore.Domain.DataBase.Models
{
    public class User
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Patronymic { get; set; } = string.Empty;
        public string Telephone { get; set; } = string.Empty;

        public required UserAdress UserAdress { get; set; }

        public required UserRegistrInfo UserRegistrInfo { get; set; }
    }
}
