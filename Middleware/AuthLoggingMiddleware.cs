using Nutra.Helper;
using System.Security.Claims;

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
        // Tenta múltiplos claim types para identificar o usuário
        var userId = context.User?.FindFirst("sub")?.Value 
                  ?? context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                  ?? context.User?.FindFirst("name")?.Value 
                  ?? context.User?.FindFirst(ClaimTypes.Email)?.Value 
                  ?? (context.User?.Identity?.IsAuthenticated == true ? "AUTHENTICATED" : "ANONYMOUS");

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
