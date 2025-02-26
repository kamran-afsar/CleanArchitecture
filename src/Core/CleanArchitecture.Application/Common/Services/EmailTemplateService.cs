public static class EmailTemplateService
{
    public static string GetWelcomeEmailTemplate(string firstName)
    {
        return $@"
            <html>
            <body>
                <h2>Welcome to Our Platform, {firstName}!</h2>
                <p>We're excited to have you on board.</p>
                <p>You can now start using our services.</p>
                <br/>
                <p>Best regards,</p>
                <p>Your Application Team</p>
            </body>
            </html>";
    }
} 