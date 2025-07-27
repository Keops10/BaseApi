namespace BaseApi.Domain.Events;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
} 