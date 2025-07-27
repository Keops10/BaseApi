using BaseApi.Abstractions.Services;
using BaseApi.Domain.Entities;
using BaseApi.Persistence.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BaseApi.Infrastructure.Services;

public class LogService : ILogService
{
    private readonly AppDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LogService(AppDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task LogInfoAsync(string message, string? source = null)
    {
        await LogAsync("INFO", message, null, source);
    }

    public async Task LogWarningAsync(string message, string? source = null)
    {
        await LogAsync("WARNING", message, null, source);
    }

    public async Task LogErrorAsync(string message, Exception? exception = null, string? source = null)
    {
        await LogAsync("ERROR", message, exception, source);
    }

    public async Task LogDebugAsync(string message, string? source = null)
    {
        await LogAsync("DEBUG", message, null, source);
    }

    public async Task LogAsync(string level, string message, Exception? exception = null, string? source = null)
    {
        try
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var userId = httpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var userName = httpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
            var ipAddress = httpContext?.Connection?.RemoteIpAddress?.ToString();
            var userAgent = httpContext?.Request?.Headers["User-Agent"].ToString();
            var requestPath = httpContext?.Request?.Path.Value;
            var requestMethod = httpContext?.Request?.Method;
            var statusCode = httpContext?.Response?.StatusCode;

            var log = new Logs(
                level: level,
                message: message,
                exception: exception?.Message,
                stackTrace: exception?.StackTrace,
                source: source ?? GetCallingMethod(),
                userId: userId,
                userName: userName,
                ipAddress: ipAddress,
                userAgent: userAgent,
                requestPath: requestPath,
                requestMethod: requestMethod,
                statusCode: statusCode
            );

            _context.Logs.Add(log);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Log servisi hata verirse, uygulamayı bozma
            Console.WriteLine($"LogService hatası: {ex.Message}");
        }
    }

    private string GetCallingMethod()
    {
        try
        {
            var stackTrace = new System.Diagnostics.StackTrace();
            var frame = stackTrace.GetFrame(3); // LogAsync'den 3 frame yukarı
            return frame?.GetMethod()?.DeclaringType?.Name ?? "Unknown";
        }
        catch
        {
            return "Unknown";
        }
    }
} 