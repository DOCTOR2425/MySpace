namespace InstrumentStore.Domain.DataBase.Models
{
    public class Brand
    {
        public Guid BrandId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
