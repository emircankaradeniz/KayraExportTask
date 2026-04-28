using MediatR;
using ProductService.Domain;
using SharedKernel;
using System;

namespace ProductService.Application.Products.Commands;

public sealed record UpdateProductCommand(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    int StockQuantity,
    ProductStatus Status) : IRequest<Result<ProductDto>>;