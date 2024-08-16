using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using ApiIsocare2.Utilities.Interface;
using Microsoft.Extensions.Configuration;


namespace ApiIsocare2.Utilities
{
    public class EmailService : IEmailService
    {
        private readonly SmtpClient _smtpClient;

        public EmailService(IConfiguration configuration)
        {
            _smtpClient = new SmtpClient(configuration["Smtp:Host"])
            {
                Port = int.Parse(configuration["Smtp:Port"]),
                Credentials = new NetworkCredential(configuration["Smtp:Username"], configuration["Smtp:Password"]),
                EnableSsl = bool.Parse(configuration["Smtp:EnableSsl"])
            };
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            

            var mailMessage = new MailMessage
            {
                From = new MailAddress("paradonapi@gmail.com", "Paradon"),
                Subject = subject,
                Body = message,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(email);

            try
            {
                await _smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                // Handle exception
                throw new InvalidOperationException($"Error sending email: {ex.Message}");
            }
        }
    }
}
