using MediatR;
using SharedKernel;

namespace ProductService.Application.Products.Commands;

public sealed record CreateProductCommand(
    string Name,
    string Description,
    string Sku,
    decimal Price,
    int StockQuantity) : IRequest<Result<ProductDto>>;