using BaseApi.Domain.Entities.Base;

namespace BaseApi.Domain.Entities;

public class Logs : BaseEntity
{
    public string Level { get; set; } = string.Empty; // INFO, WARNING, ERROR, DEBUG
    public string Message { get; set; } = string.Empty;
    public string? Exception { get; set; }
    public string? StackTrace { get; set; }
    public string? Source { get; set; }
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? RequestPath { get; set; }
    public string? RequestMethod { get; set; }
    public int? StatusCode { get; set; }
    public long? ExecutionTime { get; set; } // milliseconds
    public DateTime Timestamp { get; set; }

    private Logs() { }

    public Logs(string level, string message, string? exception = null, string? stackTrace = null,
               string? source = null, string? userId = null, string? userName = null, 
               string? ipAddress = null, string? userAgent = null, string? requestPath = null,
               string? requestMethod = null, int? statusCode = null, long? executionTime = null)
    {
        Id = Guid.NewGuid();
        Level = level;
        Message = message;
        Exception = exception;
        StackTrace = stackTrace;
        Source = source;
        UserId = userId;
        UserName = userName;
        IpAddress = ipAddress;
        UserAgent = userAgent;
        RequestPath = requestPath;
        RequestMethod = requestMethod;
        StatusCode = statusCode;
        ExecutionTime = executionTime;
        Timestamp = DateTime.UtcNow;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = userName;
    }
} 