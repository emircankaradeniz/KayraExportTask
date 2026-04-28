using Microsoft.EntityFrameworkCore;
using ProductService.Application.Abstractions;
using ProductService.Domain;
using ProductService.Infrastructure.Data;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace ProductService.Infrastructure.Repositories;

public sealed class ProductRepository : IProductRepository
{
    private readonly ProductDbContext _dbContext;

    public ProductRepository(ProductDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return _dbContext.Products
            .FirstOrDefaultAsync(product => product.Id == id && product.Status != ProductStatus.Deleted, cancellationToken);
    }

    public Task<Product?> GetBySkuAsync(string sku, CancellationToken cancellationToken)
    {
        return _dbContext.Products
            .FirstOrDefaultAsync(product => product.Sku == sku && product.Status != ProductStatus.Deleted, cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Products
            .Where(product => product.Status != ProductStatus.Deleted)
            .OrderByDescending(product => product.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Product product, CancellationToken cancellationToken)
    {
        await _dbContext.Products.AddAsync(product, cancellationToken);
    }

    public void Update(Product product)
    {
        _dbContext.Products.Update(product);
    }
}