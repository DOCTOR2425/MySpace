using System.ComponentModel.DataAnnotations;

namespace InstrumentStore.Domain.DataBase.Models
{
	public class PromoCode
	{
		public Guid PromoCodeId { get; set; }
		[MaxLength(15)]
		public string Name { get; set; } = string.Empty;
		public decimal Amount { get; set; } = 0;
		public bool IsActive { get; set; } = true;
	}
}
