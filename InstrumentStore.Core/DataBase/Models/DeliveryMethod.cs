namespace InstrumentStore.Domain.DataBase.Models
{
    public class DeliveryMethod
    {
        public int DeliveryMethodId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }

        //public DeliveryMethod(int deliveryMethodId, string name, decimal price)
        //{
        //    DeliveryMethodId = deliveryMethodId;
        //    Name = name;
        //    Price = price;
        //}
    }
}
