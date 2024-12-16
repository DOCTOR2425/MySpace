using Microsoft.EntityFrameworkCore;

namespace InstrumentStore.Domain.DataBase.Models
{
    [Keyless]
    public class CartItem
    {
        public User User { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }
}
