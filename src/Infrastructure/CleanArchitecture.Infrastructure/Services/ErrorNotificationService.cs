using CleanArchitecture.Application.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Text;
using System.Web;

namespace CleanArchitecture.Infrastructure.Services
{
    public class ErrorNotificationService : IErrorNotificationService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<ErrorNotificationService> _logger;
        private readonly IHostEnvironment _environment;
        private readonly ISendGridClient _sendGridClient;

        public ErrorNotificationService(
            IOptions<EmailSettings> emailSettings,
            ILogger<ErrorNotificationService> logger,
            IHostEnvironment environment,
            ISendGridClient sendGridClient)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
            _environment = environment;
            _sendGridClient = sendGridClient;
        }

        public async Task SendErrorNotificationAsync(Exception ex, string additionalInfo = null)
        {
            try
            {
                if (!_emailSettings.ErrorNotification.Enabled)
                {
                    return;
                }

                var subject = _emailSettings.ErrorNotification.Subject
                    .Replace("{Environment}", _environment.EnvironmentName);

                var errorMessage = BuildErrorMessage(ex, additionalInfo);

                foreach (var recipient in _emailSettings.ErrorNotification.Recipients)
                {
                    var msg = new SendGridMessage
                    {
                        From = new EmailAddress(
                            _emailSettings.SendGrid.FromEmail,
                            _emailSettings.SendGrid.FromName),
                        Subject = subject,
                        PlainTextContent = errorMessage,
                        HtmlContent = FormatErrorMessageHtml(errorMessage)
                    };

                    msg.AddTo(recipient);

                    var response = await _sendGridClient.SendEmailAsync(msg);

                    if (!response.IsSuccessStatusCode)
                    {
                        _logger.LogError("Failed to send error notification email to {Recipient}. Status Code: {StatusCode}",
                            recipient, response.StatusCode);
                    }
                }
            }
            catch (Exception notificationEx)
            {
                _logger.LogError(notificationEx, "Failed to send error notification email");
            }
        }

        private string BuildErrorMessage(Exception ex, string additionalInfo)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Error occurred at: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
            sb.AppendLine($"Environment: {_environment.EnvironmentName}");
            sb.AppendLine();

            if (!string.IsNullOrEmpty(additionalInfo))
            {
                sb.AppendLine("Additional Information:");
                sb.AppendLine(additionalInfo);
                sb.AppendLine();
            }

            sb.AppendLine("Exception Details:");
            sb.AppendLine($"Type: {ex.GetType().FullName}");
            sb.AppendLine($"Message: {ex.Message}");
            sb.AppendLine($"Source: {ex.Source}");
            sb.AppendLine();

            if (ex.StackTrace != null)
            {
                sb.AppendLine("Stack Trace:");
                sb.AppendLine(ex.StackTrace);
            }

            if (ex.InnerException != null)
            {
                sb.AppendLine();
                sb.AppendLine("Inner Exception:");
                sb.AppendLine($"Type: {ex.InnerException.GetType().FullName}");
                sb.AppendLine($"Message: {ex.InnerException.Message}");
                if (ex.InnerException.StackTrace != null)
                {
                    sb.AppendLine("Stack Trace:");
                    sb.AppendLine(ex.InnerException.StackTrace);
                }
            }

            return sb.ToString();
        }

        private string FormatErrorMessageHtml(string plainText)
        {
            return $@"
                <html>
                    <body style='font-family: Arial, sans-serif; padding: 20px;'>
                        <h2 style='color: #d32f2f;'>Application Error Alert</h2>
                        <pre style='background-color: #f5f5f5; padding: 15px; border-radius: 5px;'>
                            {HttpUtility.HtmlEncode(plainText)}
                        </pre>
                    </body>
                </html>";
        }
    }
}