using System.ComponentModel.DataAnnotations;

namespace InstrumentStore.Domain.DataBase.Models
{
    public class UserRegistrInfo
    {
        public Guid UserRegistrInfoId { get; set; }
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;
        [MaxLength(100)]
        public string PasswordHash { get; set; } = string.Empty;
        [MaxLength(500)]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
