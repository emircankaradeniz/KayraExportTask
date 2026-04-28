using MediatR;
using SharedKernel;
using System;

namespace ProductService.Application.Products.Queries;

public sealed record GetProductByIdQuery(Guid Id) : IRequest<Result<ProductDto>>;