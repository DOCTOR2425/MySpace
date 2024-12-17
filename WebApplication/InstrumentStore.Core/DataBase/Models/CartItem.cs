namespace InstrumentStore.Domain.DataBase.Models
{
    public class CartItem
    {
        public Guid CartItemId { get; set; }
        public User User { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }
}
