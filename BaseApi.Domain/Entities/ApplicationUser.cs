using Microsoft.AspNetCore.Identity;
using BaseApi.Domain.Entities.Base;

namespace BaseApi.Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>, IBaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? LastLoginDate { get; set; }
    public bool IsActive { get; set; } = true;

    public ApplicationUser()
    {
        CreatedAt = DateTime.UtcNow;
    }

    public ApplicationUser(string userName, string email, string firstName, string lastName, string? createdBy = null)
        : base(userName)
    {
        UserName = userName;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
        IsActive = true;
    }

    public void UpdateLastLogin()
    {
        LastLoginDate = DateTime.UtcNow;
    }

    public void Deactivate(string? updatedBy = null)
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void Activate(string? updatedBy = null)
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public string GetFullName()
    {
        return $"{FirstName} {LastName}".Trim();
    }
} 