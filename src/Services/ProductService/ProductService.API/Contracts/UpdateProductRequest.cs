using ProductService.Domain;

namespace ProductService.API.Contracts;

public sealed record UpdateProductRequest(
    string Name,
    string Description,
    decimal Price,
    int StockQuantity,
    ProductStatus Status);