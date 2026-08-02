using Nutra.Models;
using System.Text.Json;

namespace Nutra.Middleware;

/// <summary>
/// Único ponto de captura de exceção da API. Falha de negócio é retorno
/// (<see cref="RetornoPadrao"/>); exceção que chega aqui é bug ou infra caída,
/// e sai como 500 no mesmo envelope que o client já sabe ler.
/// </summary>
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public ExceptionMiddleware(
        RequestDelegate next,
        ILogger<ExceptionMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (UnauthorizedAccessException ex)
        {
            await EscreverRetorno(context, 401, "Usuário não autenticado.", ex);
        }
        catch (Exception ex)
        {
            var mensagem = _environment.IsDevelopment()
                ? ex.Message
                : "Erro interno do servidor.";
            await EscreverRetorno(context, 500, mensagem, ex);
        }
    }

    private async Task EscreverRetorno(HttpContext context, int statusCode, string mensagem, Exception ex)
    {
        _logger.LogError(ex, "[Nutra] {Method} {Path} => {StatusCode}",
            context.Request.Method, context.Request.Path, statusCode);

        if (context.Response.HasStarted)
        {
            _logger.LogWarning("[Nutra] Resposta já iniciada; não foi possível escrever o envelope de erro.");
            return;
        }

        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var retorno = new RetornoPadrao
        {
            Sucesso = false,
            Mensagem = mensagem,
            StatusCode = statusCode
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(retorno, JsonOpcoes));
    }

    private static readonly JsonSerializerOptions JsonOpcoes = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
}

public static class ExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionMiddleware>();
    }
}
