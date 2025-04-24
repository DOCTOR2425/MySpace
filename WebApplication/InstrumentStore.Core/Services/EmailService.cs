using System.Net;
using System.Net.Mail;
using InstrumentStore.Domain.Abstractions;
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

        public async Task SendMail(string to, string text, string subject = "")
        {
            MailAddress mailFrom = new MailAddress(_config["AdminSettings:AdminMail"], "MySpaceBy");
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
                Credentials = new NetworkCredential(mailFrom.Address, _config["AdminSettings:MailPassword"])
            };

            smtpClient.Send(message);
        }
    }
}
