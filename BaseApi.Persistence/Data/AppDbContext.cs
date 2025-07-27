using BaseApi.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using BaseApi.Abstractions.Services;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace BaseApi.Persistence.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    private readonly IHttpContextAccessor? _httpContextAccessor;

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options, IHttpContextAccessor httpContextAccessor) : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<Logs> Logs { get; set; }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var auditEntries = OnBeforeSaveChanges();
        var result = await base.SaveChangesAsync(cancellationToken);
        await OnAfterSaveChanges(auditEntries);
        await LogDatabaseOperationsAsync(auditEntries);
        return result;
    }

    private List<AuditEntry> OnBeforeSaveChanges()
    {
        ChangeTracker.DetectChanges();
        var auditEntries = new List<AuditEntry>();
        
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is AuditLog || entry.Entity is Logs || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                continue;

            var auditEntry = new AuditEntry(entry)
            {
                TableName = entry.Entity.GetType().Name,
                Action = entry.State.ToString(),
                EntityId = entry.Properties.First(p => p.Metadata.IsPrimaryKey()).CurrentValue?.ToString() ?? "",
                UserId = GetCurrentUserId(),
                UserName = GetCurrentUserName(),
                Timestamp = DateTime.UtcNow
            };

            foreach (var property in entry.Properties)
            {
                if (property.IsTemporary)
                    continue;

                auditEntry.NewValues[property.Metadata.Name] = property.CurrentValue;
                
                switch (entry.State)
                {
                    case EntityState.Added:
                        auditEntry.Action = "INSERT";
                        break;
                    case EntityState.Deleted:
                        auditEntry.Action = "DELETE";
                        auditEntry.OldValues[property.Metadata.Name] = property.OriginalValue;
                        break;
                    case EntityState.Modified:
                        if (property.IsModified)
                        {
                            auditEntry.Action = "UPDATE";
                            auditEntry.OldValues[property.Metadata.Name] = property.OriginalValue;
                            auditEntry.AffectedColumns.Add(property.Metadata.Name);
                        }
                        break;
                }
            }

            auditEntries.Add(auditEntry);
        }

        return auditEntries;
    }

    private async Task OnAfterSaveChanges(List<AuditEntry> auditEntries)
    {
        if (auditEntries == null || auditEntries.Count == 0)
            return;

        foreach (var auditEntry in auditEntries)
        {
            var auditLog = new AuditLog(
                tableName: auditEntry.TableName,
                action: auditEntry.Action,
                entityId: auditEntry.EntityId,
                oldValues: auditEntry.OldValues.Count > 0 ? JsonSerializer.Serialize(auditEntry.OldValues) : null,
                newValues: auditEntry.NewValues.Count > 0 ? JsonSerializer.Serialize(auditEntry.NewValues) : null,
                affectedColumns: auditEntry.AffectedColumns.Count > 0 ? string.Join(",", auditEntry.AffectedColumns) : null,
                userId: auditEntry.UserId,
                userName: auditEntry.UserName
            );

            AuditLogs.Add(auditLog);
        }

        // Audit log'ları da kaydet
        try
        {
            await base.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Geriye log yazma, uygulamayı bozma
            Console.WriteLine("AuditLog kaydedilemedi: " + ex.Message);
        }
    }

    private async Task LogDatabaseOperationsAsync(List<AuditEntry> auditEntries)
    {
        if (auditEntries == null || auditEntries.Count == 0)
            return;

        foreach (var auditEntry in auditEntries)
        {
            var logMessage = $"Database operation: {auditEntry.Action} on {auditEntry.TableName} (ID: {auditEntry.EntityId})";
            
            var log = new Logs(
                level: "INFO",
                message: logMessage,
                source: "AppDbContext",
                userId: auditEntry.UserId,
                userName: auditEntry.UserName
            );

            Logs.Add(log);
        }

        // Log'ları da kaydet
        try
        {
            await base.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Log servisi hata verirse, uygulamayı bozma
            Console.WriteLine("Logs kaydedilemedi: " + ex.Message);
        }
    }

    private string? GetCurrentUserId()
    {
        if (_httpContextAccessor?.HttpContext?.User?.Identity?.IsAuthenticated == true)
        {
            var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim?.Value;
        }
        return null;
    }

    private string? GetCurrentUserName()
    {
        if (_httpContextAccessor?.HttpContext?.User?.Identity?.IsAuthenticated == true)
        {
            var userNameClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Name);
            return userNameClaim?.Value;
        }
        return null;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Configure Product entity
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Price).IsRequired().HasPrecision(18, 2);
            entity.Property(e => e.Currency).IsRequired().HasMaxLength(3);
            entity.Property(e => e.Stock).IsRequired();
            entity.Property(e => e.Status).IsRequired();
            
            // Audit fields
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedAt);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            
            // Soft delete fields
            entity.Property(e => e.IsDeleted).IsRequired().HasDefaultValue(false);
            entity.Property(e => e.DeletedAt);
            entity.Property(e => e.DeletedBy).HasMaxLength(100);
            
            // Global query filter for soft delete
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Configure ApplicationUser entity
        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedAt);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.LastLoginDate);
            entity.Property(e => e.IsActive).IsRequired();

            // Unique constraints
            entity.HasIndex(e => e.UserName).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // Configure AuditLog entity
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TableName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Action).IsRequired().HasMaxLength(20);
            entity.Property(e => e.EntityId).IsRequired().HasMaxLength(50);
            entity.Property(e => e.OldValues);
            entity.Property(e => e.NewValues);
            entity.Property(e => e.AffectedColumns);
            entity.Property(e => e.UserId).HasMaxLength(50);
            entity.Property(e => e.UserName).HasMaxLength(100);
            entity.Property(e => e.IpAddress).HasMaxLength(45);
            entity.Property(e => e.UserAgent);
            entity.Property(e => e.Timestamp).IsRequired();

            // Audit fields
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedAt);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);

            // Indexes
            entity.HasIndex(e => e.TableName);
            entity.HasIndex(e => e.Action);
            entity.HasIndex(e => e.Timestamp);
            entity.HasIndex(e => e.UserId);
        });

        // Configure Logs entity
        modelBuilder.Entity<Logs>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Level).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Message).IsRequired();
            entity.Property(e => e.Exception);
            entity.Property(e => e.StackTrace);
            entity.Property(e => e.Source).HasMaxLength(100);
            entity.Property(e => e.UserId).HasMaxLength(50);
            entity.Property(e => e.UserName).HasMaxLength(100);
            entity.Property(e => e.IpAddress).HasMaxLength(45);
            entity.Property(e => e.UserAgent);
            entity.Property(e => e.RequestPath).HasMaxLength(500);
            entity.Property(e => e.RequestMethod).HasMaxLength(10);
            entity.Property(e => e.StatusCode);
            entity.Property(e => e.ExecutionTime);
            entity.Property(e => e.Timestamp).IsRequired();

            // Audit fields
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedAt);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);

            // Indexes
            entity.HasIndex(e => e.Level);
            entity.HasIndex(e => e.Timestamp);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.StatusCode);
        });
    }


}

// Audit Entry helper class
public class AuditEntry
{
    public AuditEntry(EntityEntry entry)
    {
        Entry = entry;
    }

    public EntityEntry Entry { get; }
    public string TableName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public Dictionary<string, object?> OldValues { get; set; } = new();
    public Dictionary<string, object?> NewValues { get; set; } = new();
    public List<string> AffectedColumns { get; set; } = new();
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public DateTime Timestamp { get; set; }
} 