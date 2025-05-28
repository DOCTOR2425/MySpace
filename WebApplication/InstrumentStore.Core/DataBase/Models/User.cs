using System.ComponentModel.DataAnnotations;

namespace InstrumentStore.Domain.DataBase.Models
{
	public class User
	{
		public Guid UserId { get; set; }
		[MaxLength(50)]
		public string FirstName { get; set; } = string.Empty;
		[MaxLength(50)]
		public string Surname { get; set; } = string.Empty;
		[MaxLength(21)]
		public string Telephone { get; set; } = string.Empty;
		[MaxLength(100)]
		public string Email { get; set; } = string.Empty;

		public DateTime RegistrationDate { get; set; } = DateTime.Now;
		[MaxLength(500)]
		public string RefreshToken { get; set; } = string.Empty;
		public DateTime? BlockDate { get; set; }
		[MaxLength(1000)]
		public string? BlockDetails { get; set; } = string.Empty;
	}
}
