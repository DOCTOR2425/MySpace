using System.ComponentModel.DataAnnotations;

namespace InstrumentStore.Domain.DataBase.Models
{
    public class Image
    {
        public Guid ImageId { get; set; }
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        public int Index { get; set; }

        public required Product Product { get; set; }
    }
}
