using System.ComponentModel.DataAnnotations;

namespace InstrumentStore.Domain.DataBase.Models
{
    public class City
    {
        public Guid CityId { get; set; }
        [MaxLength(30)]
        public string Name { get; set; } = string.Empty;
    }
}
