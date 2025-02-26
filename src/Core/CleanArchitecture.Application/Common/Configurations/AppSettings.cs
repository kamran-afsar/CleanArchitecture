public class JwtSettings
{
    public string Key { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public int ExpiryInMinutes { get; set; }
    public int RefreshTokenExpiryInDays { get; set; }
}

public class EmailSettings
{
    public SendGridSettings SendGrid { get; set; }
    public EmailTemplates Templates { get; set; }
}

public class SendGridSettings
{
    public string ApiKey { get; set; }
    public string FromEmail { get; set; }
    public string FromName { get; set; }
}

public class EmailTemplates
{
    public string WelcomeEmail { get; set; }
    public string PasswordReset { get; set; }
    public string EmailVerification { get; set; }
}

public class CacheSettings
{
    public RedisSettings Redis { get; set; }
}

public class RedisSettings
{
    public string ConnectionString { get; set; }
    public string InstanceName { get; set; }
    public int DefaultExpiryMinutes { get; set; }
}

public class SecuritySettings
{
    public PasswordHasherSettings PasswordHasher { get; set; }
    public bool RequireConfirmedEmail { get; set; }
    public LockoutSettings LockoutSettings { get; set; }
}

public class PasswordHasherSettings
{
    public int Iterations { get; set; }
    public int KeySize { get; set; }
}

public class LockoutSettings
{
    public int MaxFailedAttempts { get; set; }
    public int LockoutMinutes { get; set; }
}

public class DefaultCredentials
{
    public UserCredentials AdminUser { get; set; }
    public UserCredentials DefaultUser { get; set; }
}

public class UserCredentials
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}

public class FeatureSettings
{
    public bool EnableSwagger { get; set; }
    public bool EnableEmailVerification { get; set; }
    public bool EnableTwoFactorAuth { get; set; }
}

public class RateLimitSettings
{
    public bool EnableRateLimiting { get; set; }
    public int PermitLimit { get; set; }
    public TimeSpan Window { get; set; }
    public TimeSpan ReplenishmentPeriod { get; set; }
}

public class CorsSettings
{
    public string[] AllowedOrigins { get; set; }
} 