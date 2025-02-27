using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace CleanArchitecture.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(
            IOptions<EmailSettings> emailSettings,
            ILogger<EmailService> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        public Task SendEmailAsync(string to, string subject, string body)
        {
            throw new NotImplementedException();
        }

        public async Task SendWelcomeEmailAsync(string to, string userName)
        {
            var client = new SendGridClient(_emailSettings.SendGrid.ApiKey);
            var from = new EmailAddress(
                _emailSettings.SendGrid.FromEmail,
                _emailSettings.SendGrid.FromName);
            var subject = "Welcome to Clean Architecture";
            var toEmail = new EmailAddress(to);

            var templateId = _emailSettings.Templates.WelcomeEmail;
            var templateData = new
            {
                name = userName,
                date = DateTime.UtcNow.ToString("D")
            };

            var msg = MailHelper.CreateSingleTemplateEmail(
                from, toEmail, templateId, templateData);

            var response = await client.SendEmailAsync(msg);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to send welcome email to {Email}", to);
                throw new ApplicationException($"Failed to send welcome email to {to}");
            }
        }
    }
}