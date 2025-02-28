namespace InstrumentStore.Domain.DataBase.Models
{
	public class Comment
	{
		public Guid CommentId { get; set; }
		public string Text { get; set; } = string.Empty;

		public required User User { get; set; }
		public required Product Product { get; set; }
	}
}
