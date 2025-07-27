using BaseApi.Abstractions.Services;
using BaseApi.Domain.Entities;
using BaseApi.Persistence.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace BaseApi.Infrastructure.Services;

public class AuditService : IAuditService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AuditService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public AuditService(IHttpContextAccessor httpContextAccessor, ILogger<AuditService> logger, IServiceProvider serviceProvider)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public async Task LogAsync(string tableName, string action, string entityId, object? oldValues = null, 
                              object? newValues = null, string? affectedColumns = null, string? userId = null, 
                              string? userName = null)
    {
        try
        {
            _logger.LogInformation("AuditService.LogAsync called: {TableName} {Action} {EntityId}", tableName, action, entityId);
            
            var httpContext = _httpContextAccessor.HttpContext;
            var ipAddress = httpContext?.Connection?.RemoteIpAddress?.ToString();
            var userAgent = httpContext?.Request?.Headers["User-Agent"].ToString();

            var auditLog = new AuditLog(
                tableName: tableName,
                action: action,
                entityId: entityId,
                oldValues: oldValues != null ? JsonSerializer.Serialize(oldValues) : null,
                newValues: newValues != null ? JsonSerializer.Serialize(newValues) : null,
                affectedColumns: affectedColumns,
                userId: userId,
                userName: userName,
                ipAddress: ipAddress,
                userAgent: userAgent
            );

            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            
            context.AuditLogs.Add(auditLog);
            await context.SaveChangesAsync();

            _logger.LogInformation("Audit log created: {TableName} {Action} {EntityId}", tableName, action, entityId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating audit log: {TableName} {Action} {EntityId} - Error: {Error}", tableName, action, entityId, ex.Message);
        }
    }
} 