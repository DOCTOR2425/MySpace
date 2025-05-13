using System.ComponentModel.DataAnnotations;

namespace InstrumentStore.Domain.DataBase.Models
{
	public class User
	{
		public Guid UserId { get; set; }
		[MaxLength(100)]
		public string FirstName { get; set; } = string.Empty;
		[MaxLength(100)]
		public string Surname { get; set; } = string.Empty;
		[MaxLength(21)]
		public string Telephone { get; set; } = string.Empty;
		[MaxLength(255)]
		public string Email { get; set; } = string.Empty;
		[MaxLength(500)]
		public string RefreshToken { get; set; } = string.Empty;
	}
}
