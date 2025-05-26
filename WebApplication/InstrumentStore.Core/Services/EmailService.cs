using InstrumentStore.Domain.Abstractions;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

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
			// Проверка валидности email
			if (!IsValidEmail(to))
				throw new ArgumentException("Некорректный email адрес", nameof(to));

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

			try
			{
				// Попытка отправить письмо
				await smtpClient.SendMailAsync(message);
			}
			catch (SmtpFailedRecipientException ex)
			{
				// Ловим специфическое исключение для несуществующего адреса
				if (ex.StatusCode == SmtpStatusCode.MailboxUnavailable ||
					ex.StatusCode == SmtpStatusCode.MailboxNameNotAllowed)
				{
					throw new InvalidOperationException($"Адрес электронной почты не существует: {to}", ex);
				}
				throw; // Перебрасываем другие исключения
			}
			catch (SmtpException ex)
			{
				// Общая обработка ошибок SMTP
				throw new InvalidOperationException($"Ошибка при отправке письма: {ex.Message}", ex);
			}
			finally
			{
				message.Dispose();
				smtpClient.Dispose();
			}
		}

		private bool IsValidEmail(string email)
		{
			if (string.IsNullOrWhiteSpace(email))
				return false;

			try
			{
				// Используем MailAddress для проверки формата
				var addr = new MailAddress(email);
				return addr.Address == email;
			}
			catch
			{
				return false;
			}
		}
	}
}
