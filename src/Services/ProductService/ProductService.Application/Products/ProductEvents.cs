using System;

namespace ProductService.Application.Products;

public sealed record ProductCreatedIntegrationEvent(
    Guid ProductId,
    string Name,
    decimal Price,
    DateTime OccurredOnUtc);

public sealed record ProductUpdatedIntegrationEvent(
    Guid ProductId,
    string Name,
    decimal Price,
    DateTime OccurredOnUtc);