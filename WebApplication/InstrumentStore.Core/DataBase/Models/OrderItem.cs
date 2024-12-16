using Microsoft.EntityFrameworkCore;

namespace InstrumentStore.Domain.DataBase.Models
{
    [Keyless]
    public class OrderItem
    {
        public Order tbl_Order { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }
}
