using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class ConfigurationService
{
    public static IServiceCollection AddApplicationConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Bind configuration sections to strongly typed classes
        services.Configure<JwtSettings>(
            configuration.GetSection("Jwt"));
            
        services.Configure<EmailSettings>(
            configuration.GetSection("Email"));
            
        services.Configure<CacheSettings>(
            configuration.GetSection("Cache"));
            
        services.Configure<SecuritySettings>(
            configuration.GetSection("Security"));
            
        services.Configure<DefaultCredentials>(
            configuration.GetSection("DefaultCredentials"));
            
        services.Configure<FeatureSettings>(
            configuration.GetSection("Features"));
            
        services.Configure<RateLimitSettings>(
            configuration.GetSection("RateLimiting"));
            
        services.Configure<CorsSettings>(
            configuration.GetSection("Cors"));

        return services;
    }
} 