using System.ComponentModel.DataAnnotations;

namespace InstrumentStore.Domain.DataBase.Models
{
    public class ProductCategory
    {
        [MaxLength(100)]
        public Guid ProductCategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
