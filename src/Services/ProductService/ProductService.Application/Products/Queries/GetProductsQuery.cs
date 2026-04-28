using MediatR;
using SharedKernel;

namespace ProductService.Application.Products.Queries;

public sealed record GetProductsQuery : IRequest<Result<IReadOnlyList<ProductDto>>>;