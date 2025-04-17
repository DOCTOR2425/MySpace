namespace InstrumentStore.Domain.Contracts.Comment
{
    public class CommentForUserResponse
    {
        public string Text { get; set; } = string.Empty;
        public DateTime CreationDate { get; set; }

        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
    }
}
