using SharedKernel;
using System;

namespace ProductService.Domain;

public sealed class ProductCreatedDomainEvent : IDomainEvent
{
    public ProductCreatedDomainEvent(Guid productId, string name, decimal price)
    {
        EventId = Guid.NewGuid();
        OccurredOnUtc = DateTime.UtcNow;
        ProductId = productId;
        Name = name;
        Price = price;
    }

    public Guid EventId { get; }

    public DateTime OccurredOnUtc { get; }

    public Guid ProductId { get; }

    public string Name { get; }

    public decimal Price { get; }
}