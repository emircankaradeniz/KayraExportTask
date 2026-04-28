using System;
using System.Text.Json;
using System.Threading.Tasks;
using System.Threading;
using ProductService.Application.Abstractions;
using ProductService.Application.Products;
using StackExchange.Redis;

namespace ProductService.Infrastructure.Caching;

public sealed class ProductCacheService : IProductCacheService
{
    private const string ProductsCacheKey = "products:list";
    private readonly IDatabase _database;

    public ProductCacheService(IConnectionMultiplexer connectionMultiplexer)
    {
        _database = connectionMultiplexer.GetDatabase();
    }

    public async Task<IReadOnlyList<ProductDto>?> GetProductsAsync(CancellationToken cancellationToken)
    {
        var cachedProducts = await _database.StringGetAsync(ProductsCacheKey);

        if (cachedProducts.IsNullOrEmpty)
        {
            return null;
        }

        return JsonSerializer.Deserialize<IReadOnlyList<ProductDto>>(cachedProducts!);
    }

    public async Task SetProductsAsync(IReadOnlyList<ProductDto> products, CancellationToken cancellationToken)
    {
        var serializedProducts = JsonSerializer.Serialize(products);

        await _database.StringSetAsync(
            ProductsCacheKey,
            serializedProducts,
            TimeSpan.FromMinutes(10));
    }

    public async Task RemoveProductsAsync(CancellationToken cancellationToken)
    {
        await _database.KeyDeleteAsync(ProductsCacheKey);
    }
}