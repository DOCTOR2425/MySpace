using System.ComponentModel.DataAnnotations;

namespace InstrumentStore.Domain.DataBase.Models
{
    public class Country
    {
        public Guid CountryId { get; set; }
        [MaxLength(30)]
        public string Name { get; set; } = string.Empty;
    }
}
