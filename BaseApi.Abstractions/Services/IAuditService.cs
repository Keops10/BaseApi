namespace BaseApi.Abstractions.Services;

public interface IAuditService
{
    Task LogAsync(string tableName, string action, string entityId, object? oldValues = null, 
                 object? newValues = null, string? affectedColumns = null, string? userId = null, 
                 string? userName = null);
} 