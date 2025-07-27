namespace BaseApi.Abstractions.Services;

public interface ILogService
{
    Task LogInfoAsync(string message, string? source = null);
    Task LogWarningAsync(string message, string? source = null);
    Task LogErrorAsync(string message, Exception? exception = null, string? source = null);
    Task LogDebugAsync(string message, string? source = null);
    Task LogAsync(string level, string message, Exception? exception = null, string? source = null);
} 