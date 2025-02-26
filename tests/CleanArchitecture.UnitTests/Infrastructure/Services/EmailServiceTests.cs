using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xunit;
using FluentAssertions;
using Moq;

namespace CleanArchitecture.UnitTests.Infrastructure.Services
{
    public class EmailServiceTests
    {
        private readonly Mock<IOptions<EmailSettings>> _mockEmailSettings;
        private readonly Mock<ILogger<EmailService>> _mockLogger;
        private readonly EmailService _emailService;

        public EmailServiceTests()
        {
            _mockEmailSettings = new Mock<IOptions<EmailSettings>>();
            _mockLogger = new Mock<ILogger<EmailService>>();

            _mockEmailSettings.Setup(x => x.Value).Returns(new EmailSettings
            {
                SendGrid = new SendGridSettings
                {
                    ApiKey = "test-api-key",
                    FromEmail = "test@example.com",
                    FromName = "Test Sender"
                }
            });

            _emailService = new EmailService(_mockEmailSettings.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task SendEmailAsync_WithValidInput_ShouldSendEmail()
        {
            // Arrange
            var to = "recipient@example.com";
            var subject = "Test Subject";
            var body = "Test Body";

            // Act & Assert
            await _emailService.Invoking(x => x.SendEmailAsync(to, subject, body))
                .Should().NotThrowAsync();
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("invalid-email")]
        public async Task SendEmailAsync_WithInvalidEmail_ShouldThrowArgumentException(string invalidEmail)
        {
            // Arrange
            var subject = "Test Subject";
            var body = "Test Body";

            // Act & Assert
            await _emailService.Invoking(x => x.SendEmailAsync(invalidEmail, subject, body))
                .Should().ThrowAsync<ArgumentException>();
        }
    }
} 