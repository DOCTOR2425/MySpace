namespace InstrumentStore.Domain.DataBase.Models
{
	public class UserRegistrInfo
	{
		public Guid UserRegistrInfoId { get; set; }
		public string Email { get; set; } = string.Empty;
		public string PasswordHash { get; set; } = string.Empty;
		public string RefreshToken { get; set; } = string.Empty;
	}
}
