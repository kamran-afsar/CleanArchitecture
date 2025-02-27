using CleanArchitecture.Application.Interfaces;
using SendGrid.Helpers.Errors.Model;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text;

namespace CleanArchitecture.Api.Middleware
{
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
        private readonly IHostEnvironment _environment;
        private readonly IServiceScopeFactory _scopeFactory;

        public GlobalExceptionHandlerMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionHandlerMiddleware> logger,
            IHostEnvironment environment,
            IServiceScopeFactory scopeFactory) // Inject scope factory instead of IErrorNotificationService
        {
            _next = next;
            _logger = logger;
            _environment = environment;
            _scopeFactory = scopeFactory;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred");

                var requestInfo = await BuildRequestInfoAsync(context);

                // Create a new scope to resolve the scoped IErrorNotificationService
                using (var scope = _scopeFactory.CreateScope())
                {
                    var errorNotificationService = scope.ServiceProvider.GetRequiredService<IErrorNotificationService>();
                    await errorNotificationService.SendErrorNotificationAsync(ex, requestInfo);
                }

                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task<string> BuildRequestInfoAsync(HttpContext context)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Request Information:");
            sb.AppendLine($"Timestamp: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
            sb.AppendLine($"TraceId: {Activity.Current?.Id ?? context.TraceIdentifier}");
            sb.AppendLine($"Method: {context.Request.Method}");
            sb.AppendLine($"Path: {context.Request.Path}");
            sb.AppendLine($"QueryString: {context.Request.QueryString}");

            // Add request headers
            sb.AppendLine("\nHeaders:");
            foreach (var header in context.Request.Headers)
            {
                if (!header.Key.ToLower().Contains("authorization")) // Skip sensitive headers
                {
                    sb.AppendLine($"{header.Key}: {header.Value}");
                }
            }

            // Add request body for POST/PUT/PATCH
            if (context.Request.Method.ToUpper() is "POST" or "PUT" or "PATCH")
            {
                try
                {
                    context.Request.EnableBuffering();
                    using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
                    var body = await reader.ReadToEndAsync();
                    context.Request.Body.Position = 0; // Reset position

                    if (!string.IsNullOrEmpty(body))
                    {
                        sb.AppendLine("\nRequest Body:");
                        sb.AppendLine(body);
                    }
                }
                catch (Exception ex)
                {
                    sb.AppendLine($"\nError reading request body: {ex.Message}");
                }
            }

            // Add user information if authenticated
            if (context.User?.Identity?.IsAuthenticated == true)
            {
                sb.AppendLine("\nUser Information:");
                sb.AppendLine($"Username: {context.User.Identity.Name}");
                sb.AppendLine($"Claims: {string.Join(", ", context.User.Claims.Select(c => $"{c.Type}={c.Value}"))}");
            }

            return sb.ToString();
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";

            var response = new ErrorResponse
            {
                TraceId = Activity.Current?.Id ?? context.TraceIdentifier,
                ExceptionType = ex.GetType().Name
            };

            switch (ex)
            {
                case ValidationException validationEx:
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    response.Message = "Validation failed";
                    break;

                case UnauthorizedException:
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    response.Message = "Unauthorized";
                    break;

                case NotFoundException notFoundEx:
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    response.Message = notFoundEx.Message;
                    break;

                case BadRequestException badRequestEx:
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    response.Message = badRequestEx.Message;
                    break;

                default:
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    response.Message = _environment.IsDevelopment()
                        ? ex.Message
                        : "An unexpected error occurred. Please try again later.";
                    break;
            }

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
