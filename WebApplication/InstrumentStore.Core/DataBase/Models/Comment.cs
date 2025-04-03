using System.ComponentModel.DataAnnotations;

namespace InstrumentStore.Domain.DataBase.Models
{
    public class Comment
    {
        public Guid CommentId { get; set; }
        [MaxLength(1000)]
        public string Text { get; set; } = string.Empty;
        public DateTime CreationDate { get; set; }

        public required User User { get; set; }
        public required Product Product { get; set; }
    }
}
