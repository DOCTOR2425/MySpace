namespace InstrumentStore.Domain.DataBase.Models
{
    public class PaidOrderItem
    {
        public Guid PaidOrderItemId { get; set; }
        public PaidOrder PaidOrder { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }
}
