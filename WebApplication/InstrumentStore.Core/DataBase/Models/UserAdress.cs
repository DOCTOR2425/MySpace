namespace InstrumentStore.Domain.DataBase.Models
{
    public class UserAdress
    {
        public Guid UserAdressId { get; set; }
        public string City { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string HouseNumber { get; set; } = string.Empty;
        public string Entrance { get; set; } = string.Empty;
        public string Flat { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"{City} ул.{Street} дом {HouseNumber} кв.{Flat} (подъезд {Entrance})";
        }
    }
}
