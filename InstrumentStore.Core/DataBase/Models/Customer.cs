namespace InstrumentStore.Domain.DataBase.Models
{
    public class Customer
    {
        public Guid CustomerId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Patronymic { get; set; } = string.Empty;
        public string Telephone { get; set; } = string.Empty;
        public string EMail { get; set; } = string.Empty;

        public required CustomerAdress CustomerAdress { get; set; }
    }
}
