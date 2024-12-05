namespace InstrumentStore.Domain.DataBase.Models
{
    public class CustomerAdress
    {
        public int CustomerAdressId { get; set; }
        public string City { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string Entrance { get; set; } = string.Empty;
        public string Flat { get; set; } = string.Empty;
    }
}
