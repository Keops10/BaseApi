using BaseApi.Domain.Enums;
using BaseApi.Domain.Events;
using BaseApi.Domain.Entities.Base;

namespace BaseApi.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public decimal Price { get; private set; }
    public string Currency { get; private set; }
    public int Stock { get; private set; }
    public ProductStatus Status { get; private set; }

    private Product() { } // EF Core için

    public Product(string name, string description, decimal price, string currency, int stock, string? createdBy = null)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        Price = price;
        Currency = currency;
        Stock = stock;
        Status = ProductStatus.Active;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;

        // Domain Event'i tetikle
        AddDomainEvent(new ProductCreatedEvent(this));
    }

    public void UpdateStock(int newStock, string? updatedBy = null)
    {
        if (newStock < 0)
            throw new ArgumentException("Stock cannot be negative");

        Stock = newStock;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void UpdatePrice(decimal newPrice, string currency, string? updatedBy = null)
    {
        Price = newPrice;
        Currency = currency;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void Deactivate(string? updatedBy = null)
    {
        Status = ProductStatus.Inactive;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void Activate(string? updatedBy = null)
    {
        Status = ProductStatus.Active;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void SoftDelete(string? deletedBy = null)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
        Status = ProductStatus.Inactive;
    }

    public void Restore(string? restoredBy = null)
    {
        IsDeleted = false;
        DeletedAt = null;
        DeletedBy = null;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = restoredBy;
    }
} 