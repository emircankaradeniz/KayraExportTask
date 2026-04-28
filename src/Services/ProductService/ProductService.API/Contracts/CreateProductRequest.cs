namespace ProductService.API.Contracts;

public sealed record CreateProductRequest(
    string Name,
    string Description,
    string Sku,
    decimal Price,
    int StockQuantity);