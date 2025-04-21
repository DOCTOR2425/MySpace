using System.ComponentModel.DataAnnotations;

namespace InstrumentStore.Domain.DataBase.Models
{
    public class Brand
    {
        public Guid BrandId { get; set; }
        [MaxLength(30)]
        public string Name { get; set; } = string.Empty;
    }
}
