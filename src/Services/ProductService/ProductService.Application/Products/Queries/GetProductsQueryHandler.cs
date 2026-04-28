using MediatR;
using ProductService.Application.Abstractions;
using SharedKernel;
using System.Threading.Tasks;
using System.Threading;

namespace ProductService.Application.Products.Queries;

public sealed class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, Result<IReadOnlyList<ProductDto>>>
{
    private readonly IProductRepository _productRepository;
    private readonly IProductCacheService _productCacheService;

    public GetProductsQueryHandler(
        IProductRepository productRepository,
        IProductCacheService productCacheService)
    {
        _productRepository = productRepository;
        _productCacheService = productCacheService;
    }

    public async Task<Result<IReadOnlyList<ProductDto>>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var cachedProducts = await _productCacheService.GetProductsAsync(cancellationToken);

        if (cachedProducts is not null)
        {
            return Result.Success(cachedProducts);
        }

        var products = await _productRepository.GetAllAsync(cancellationToken);
        var productDtos = products.Select(product => product.ToDto()).ToList();

        await _productCacheService.SetProductsAsync(productDtos, cancellationToken);

        return Result.Success<IReadOnlyList<ProductDto>>(productDtos);
    }
}