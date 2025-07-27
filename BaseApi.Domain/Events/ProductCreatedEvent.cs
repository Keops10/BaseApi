using BaseApi.Domain.Entities;

namespace BaseApi.Domain.Events;

public class ProductCreatedEvent : IDomainEvent
{
    public Product Product { get; }
    public DateTime OccurredOn { get; }

    public ProductCreatedEvent(Product product)
    {
        Product = product;
        OccurredOn = DateTime.UtcNow;
    }
} 