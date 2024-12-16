namespace InstrumentStore.Domain.DataBase.Models
{
    public class PaidOrder
    {
        public Guid PaidOrderId { get; set; }
        public DateTime PaymentDate { get; set; }

        public required DeliveryMethod DeliveryMethod { get; set; }
        public required PaymentMethod PaymentMethod { get; set; }
        public required User User { get; set; }
    }
}
