namespace InstrumentStore.Domain.Contracts.Comment
{
    public class UserCommentResponse
    {
        public string Text { get; set; } = string.Empty;
        public DateTime CreationDate { get; set; }
        public string Image { get; set; } = string.Empty;

        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
    }
}
