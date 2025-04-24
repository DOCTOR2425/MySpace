namespace InstrumentStore.Domain.Abstractions
{
    public interface IEmailService
    {
        Task SendMail(string to, string text, string subject = "");
    }
}