namespace InstrumentStore.Domain.DataBase.Models
{
    public class PaymentMethod
    {
        public Guid PaymentMethodId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
