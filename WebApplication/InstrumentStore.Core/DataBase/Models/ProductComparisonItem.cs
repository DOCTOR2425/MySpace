namespace InstrumentStore.Domain.DataBase.Models
{
    public class ProductComparisonItem
    {
        public Guid ProductComparisonItemId { get; set; }
        public Product Product { get; set; }
        public User User { get; set; }
    }
}
