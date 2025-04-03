namespace InstrumentStore.Domain.Contracts.Comment
{
    public class CreateCommentRequest
    {
        public string Text { get; set; } = string.Empty;
        public Guid ProductId { get; set; }
    }
}
