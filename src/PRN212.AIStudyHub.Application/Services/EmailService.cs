using Microsoft.Extensions.Configuration;
using PRN212.AIStudyHub.Application.Interfaces;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace PRN212.AIStudyHub.Application.Services
{
  public class EmailService : IEmailService
  {
	private readonly IConfiguration _config;

	public EmailService(IConfiguration config)
	{
	  _config = config;
	}

	public async Task SendEmailAsync(string toEmail, string subject, string body)
	{
	  var smtpServer = _config["EmailSettings:SmtpServer"];
	  var port = int.Parse(_config["EmailSettings:SmtpPort"] ?? "587");
	  var senderEmail = _config["EmailSettings:SenderEmail"];
	  var password = _config["EmailSettings:SenderPassword"]?.Trim();
	  var senderName = _config["EmailSettings:SenderName"] ?? "AI Study Hub";

	  if (string.IsNullOrEmpty(smtpServer) || string.IsNullOrEmpty(senderEmail) || string.IsNullOrEmpty(password))
	  {
		throw new InvalidOperationException("Email settings are not configured properly in appsettings.json.");
	  }

	  var message = new MailMessage
	  {
		From = new MailAddress(senderEmail, senderName),
		Subject = subject,
		Body = body,
		IsBodyHtml = true
	  };
	  message.To.Add(new MailAddress(toEmail));

	  using var smtpClient = new SmtpClient(smtpServer, port)
	  {
		Credentials = new NetworkCredential(senderEmail, password),
		EnableSsl = true
	  };

	  await smtpClient.SendMailAsync(message);
	}
  }
}
