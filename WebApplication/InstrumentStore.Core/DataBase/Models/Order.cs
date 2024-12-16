namespace InstrumentStore.Domain.DataBase.Models
{
    public class Order
    {
        public Guid OrderId { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime DeliveryDate { get; set; }

        public required DeliveryMethod DeliveryMethod { get; set; }
        public required PaymentMethod PaymentMethod { get; set; }
        public required User User { get; set; }
    }
}
