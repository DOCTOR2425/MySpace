using InstrumentStore.Domain.Abstractions;
using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Configuration;

namespace InstrumentStore.Domain.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public void SendMail(string to, string text, string subject = "")
        {
            MailAddress mailFrom = new MailAddress(_config["AdminMail:MySpaceMail"], "MySpaceBy");
            MailAddress mailTo = new MailAddress(to);
            MailMessage message = new MailMessage(mailFrom, mailTo);
            message.Body = text;
            message.Subject = subject;

            SmtpClient smtpClient = new SmtpClient()
            {
                Host = "smtp.mail.ru",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(mailFrom.Address, _config.GetValue<string>("MailPassword"))
            };

            smtpClient.Send(message);
        }
    }
}
