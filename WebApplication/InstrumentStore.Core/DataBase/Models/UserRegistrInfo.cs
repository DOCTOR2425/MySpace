namespace InstrumentStore.Domain.DataBase.Models
{
    public class UserRegistrInfo
    {
        public Guid UserRegistrInfoId { get; set; }
        public string EMail { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
    }
}
