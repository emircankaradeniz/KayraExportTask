using MediatR;
using ProductService.Application.Abstractions;
using ProductService.Domain;
using SharedKernel;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace ProductService.Application.Products.Commands;

public sealed class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<ProductDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProductCacheService _productCacheService;
    private readonly IEventBus _eventBus;

    public CreateProductCommandHandler(
        IProductRepository productRepository,
        IUnitOfWork unitOfWork,
        IProductCacheService productCacheService,
        IEventBus eventBus)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _productCacheService = productCacheService;
        _eventBus = eventBus;
    }

    public async Task<Result<ProductDto>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var existingProduct = await _productRepository.GetBySkuAsync(request.Sku, cancellationToken);

        if (existingProduct is not null)
        {
            return Result.Failure<ProductDto>("A product with the same SKU already exists.");
        }

        Product product;

        try
        {
            product = Product.Create(
                request.Name,
                request.Description,
                request.Sku,
                request.Price,
                request.StockQuantity);
        }
        catch (ArgumentException exception)
        {
            return Result.Failure<ProductDto>(exception.Message);
        }

        await _productRepository.AddAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _productCacheService.RemoveProductsAsync(cancellationToken);

        await _eventBus.PublishAsync(
            new ProductCreatedIntegrationEvent(
                product.Id,
                product.Name,
                product.Price,
                DateTime.UtcNow),
            cancellationToken);

        return Result.Success(product.ToDto());
    }
}