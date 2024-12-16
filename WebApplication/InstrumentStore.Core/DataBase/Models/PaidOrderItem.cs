using Microsoft.EntityFrameworkCore;

namespace InstrumentStore.Domain.DataBase.Models
{
    [Keyless]
    public class PaidOrderItem
    {
        public PaidOrder PaidOrder { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }
}
