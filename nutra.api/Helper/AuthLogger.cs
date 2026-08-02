using System;
using System.IO;
using System.Text;

namespace Nutra.Helper;

/// <summary>
/// Serviço de logging dedicado para rastrear fluxos de autenticação.
/// Escreve em arquivo e console simultaneamente para debug.
/// </summary>
public class AuthLogger
{
    private readonly ILogger<AuthLogger> _logger;
    private readonly string _logDirectory;
    private readonly string _logFilePath;

    public AuthLogger(ILogger<AuthLogger> logger, IWebHostEnvironment env)
    {
        _logger = logger;
        _logDirectory = Path.Combine(env.ContentRootPath, "logs", "auth");
        
        // Cria diretório se não existir
        Directory.CreateDirectory(_logDirectory);
        
        // Arquivo de log diário
        var dateString = DateTime.Now.ToString("yyyy-MM-dd");
        _logFilePath = Path.Combine(_logDirectory, $"auth-{dateString}.log");
    }

    public void LogAuthStart(string userId, string method, string path)
    {
        var message = $"[AUTH-START] User: {userId ?? "ANONYMOUS"} | Method: {method} | Path: {path}";
        WriteLog(message, LogLevel.Information);
    }

    public void LogAuthStep(string step, string details, string userId = null)
    {
        var message = $"[AUTH-STEP] {step} | User: {userId ?? "ANONYMOUS"} | Details: {details}";
        WriteLog(message, LogLevel.Information);
    }

    public void LogRedirect(string from, string to, string userId = null, string reason = null)
    {
        var message = $"[AUTH-REDIRECT] From: {from} → To: {to} | User: {userId ?? "ANONYMOUS"} | Reason: {reason ?? "N/A"}";
        WriteLog(message, LogLevel.Information);
    }

    public void LogOpenIdEvent(string eventName, string details, string userId = null)
    {
        var message = $"[OPENID-EVENT] {eventName} | User: {userId ?? "ANONYMOUS"} | Details: {details}";
        WriteLog(message, LogLevel.Information);
    }

    public void LogException(string context, Exception ex, string userId = null)
    {
        var message = $"[AUTH-ERROR] Context: {context} | User: {userId ?? "ANONYMOUS"} | Exception: {ex.Message}\nStackTrace: {ex.StackTrace}";
        WriteLog(message, LogLevel.Error);
    }

    public void LogWarning(string context, string message, string userId = null)
    {
        var formattedMessage = $"[AUTH-WARNING] Context: {context} | User: {userId ?? "ANONYMOUS"} | Message: {message}";
        WriteLog(formattedMessage, LogLevel.Warning);
    }

    private void WriteLog(string message, LogLevel level)
    {
        var timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff");
        var levelString = level.ToString().ToUpper();
        var formattedMessage = $"[{timestamp}] [{levelString}] {message}";

        // Log no console
        _logger.Log(level, formattedMessage);

        // Log em arquivo
        try
        {
            lock (this)
            {
                File.AppendAllText(_logFilePath, formattedMessage + Environment.NewLine, Encoding.UTF8);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao escrever no arquivo de log");
        }
    }

    public string GetLogFile()
    {
        return _logFilePath;
    }

    public string GetLogDirectory()
    {
        return _logDirectory;
    }
}
