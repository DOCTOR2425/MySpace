using System.ComponentModel.DataAnnotations;

namespace InstrumentStore.Domain.DataBase.Models
{
    public class DeliveryMethod
    {
        public Guid DeliveryMethodId { get; set; }
        [MaxLength(30)]
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}
