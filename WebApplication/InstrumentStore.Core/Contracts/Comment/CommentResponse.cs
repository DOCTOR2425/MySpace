namespace InstrumentStore.Domain.Contracts.Comment
{
    public class CommentResponse
    {
        public string Text { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public DateTime CreationDate { get; set; }
    }
}
