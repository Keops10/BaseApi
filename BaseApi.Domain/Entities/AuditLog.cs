using BaseApi.Domain.Entities.Base;

namespace BaseApi.Domain.Entities;

public class AuditLog : BaseEntity
{
    public string TableName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty; // INSERT, UPDATE, DELETE
    public string EntityId { get; set; } = string.Empty;
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string? AffectedColumns { get; set; }
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public DateTime Timestamp { get; set; }

    private AuditLog() { }

    public AuditLog(string tableName, string action, string entityId, string? oldValues = null, 
                   string? newValues = null, string? affectedColumns = null, string? userId = null, 
                   string? userName = null, string? ipAddress = null, string? userAgent = null)
    {
        Id = Guid.NewGuid();
        TableName = tableName;
        Action = action;
        EntityId = entityId;
        OldValues = oldValues;
        NewValues = newValues;
        AffectedColumns = affectedColumns;
        UserId = userId;
        UserName = userName;
        IpAddress = ipAddress;
        UserAgent = userAgent;
        Timestamp = DateTime.UtcNow;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = userName;
    }
} 