namespace InstrumentStore.Domain.DataBase.Models
{
    public class DeliveryMethod
    {
        public Guid DeliveryMethodId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}
