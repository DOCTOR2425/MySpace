namespace InstrumentStore.Domain.DataBase.Models
{
    public class tbl_Order
    {
        public Guid tbl_OrderId { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime DeliveryDate { get; set; }

        public required DeliveryMethod DeliveryMethod { get; set; }
        public required PaymentMethod PaymentMethod { get; set; }
        public required Customer Customer { get; set; }
    }
}
