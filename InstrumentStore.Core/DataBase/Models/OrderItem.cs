using Microsoft.EntityFrameworkCore;

namespace InstrumentStore.Domain.DataBase.Models
{
    [Keyless]
    public class OrderItem
    {
        public int OrderId { get; set; }
        public int InstrumentId { get; set; }
        public int Quantity { get; set; }

        //public OrderItem(int orderId, int instrumentId, int quantity)
        //{
        //    OrderId = orderId;
        //    InstrumentId = instrumentId;
        //    Quantity = quantity;
        //}
    }
}
