using ProductService.Domain;
using System;

namespace ProductService.Application.Products;

public sealed record ProductDto(
    Guid Id,
    string Name,
    string Description,
    string Sku,
    decimal Price,
    int StockQuantity,
    ProductStatus Status,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);