using ProductService.Application.Products;
using System.Threading.Tasks;
using System.Threading;

namespace ProductService.Application.Abstractions;

public interface IProductCacheService
{
    Task<IReadOnlyList<ProductDto>?> GetProductsAsync(CancellationToken cancellationToken);

    Task SetProductsAsync(IReadOnlyList<ProductDto> products, CancellationToken cancellationToken);

    Task RemoveProductsAsync(CancellationToken cancellationToken);
}