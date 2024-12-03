namespace InstrumentStore.Domain.DataBase.Models
{
    public class tbl_Order
    {
        public int tbl_OrderId { get; set; }
        public string CustomerCity { get; set; }
        public string CustomerAddress { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public int DeliveryMethodId { get; set; }
        public int PaymentMethodId { get; set; }
        public int CustomerId { get; set; }

        //public tbl_Order(int orderId, string customerCity,
        //    string customerAddress, DateTime registrationDate,
        //    DateTime deliveryDate, int deliveryMethod,
        //    int paymentMethod, int customer)
        //{
        //    OrderId = orderId;
        //    CustomerCity = customerCity;
        //    CustomerAddress = customerAddress;
        //    RegistrationDate = registrationDate;
        //    DeliveryDate = deliveryDate;
        //    DeliveryMethod = deliveryMethod;
        //    PaymentMethod = paymentMethod;
        //    Customer = customer;
        //}
    }
}
