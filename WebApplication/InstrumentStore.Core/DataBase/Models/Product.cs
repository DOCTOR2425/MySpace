using System.ComponentModel.DataAnnotations;

namespace InstrumentStore.Domain.DataBase.Models
{
    public class Product
    {
        public Guid ProductId { get; set; }
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;
        [MaxLength(1500)]
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public bool IsArchive { get; set; } = false;

        public Brand Brand { get; set; }
        public Country Country { get; set; }
        public ProductCategory ProductCategory { get; set; }
    }
}
