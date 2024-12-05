namespace InstrumentStore.Domain.DataBase.Models
{
    public class tbl_Order
    {
        public int tbl_OrderId { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime DeliveryDate { get; set; }

        public DeliveryMethod DeliveryMethod { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public Customer Customer { get; set; }
    }
}
