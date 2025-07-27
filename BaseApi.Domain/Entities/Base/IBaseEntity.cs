namespace BaseApi.Domain.Entities.Base;

public interface IBaseEntity
{
    Guid Id { get; }
    DateTime CreatedAt { get; set; }
    string? CreatedBy { get; set; }
    DateTime? UpdatedAt { get; set; }
    string? UpdatedBy { get; set; }
} 