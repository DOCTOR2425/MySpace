namespace InstrumentStore.Domain.Abstractions
{
    public interface IEmailService
    {
        void SendMail(string to, string text, string subject = "");
    }
}