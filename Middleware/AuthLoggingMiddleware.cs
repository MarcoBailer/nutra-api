using Nutra.Helper;

namespace Nutra.Middleware;

public class AuthLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly AuthLogger _authLogger;

    public AuthLoggingMiddleware(RequestDelegate next, AuthLogger authLogger)
    {
        _next = next;
        _authLogger = authLogger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var userId = context.User?.FindFirst("sub")?.Value ?? context.User?.FindFirst("email")?.Value ?? "ANONYMOUS";

        // Log de entrada
        _authLogger.LogAuthStart(userId, context.Request.Method, context.Request.Path);

        // Chama o próximo middleware
        await _next(context);

        // Log de saída
        var statusCode = context.Response.StatusCode;
        _authLogger.LogAuthStep("REQUEST-COMPLETED", $"Status Code: {statusCode}", userId);
    }
}

public static class AuthLoggingMiddlewareExtensions
{
    public static IApplicationBuilder UseAuthLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<AuthLoggingMiddleware>();
    }
}
