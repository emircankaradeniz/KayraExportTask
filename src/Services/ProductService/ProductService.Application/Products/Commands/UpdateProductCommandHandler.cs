using MediatR;
using ProductService.Application.Abstractions;
using SharedKernel;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace ProductService.Application.Products.Commands;

public sealed class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Result<ProductDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProductCacheService _productCacheService;
    private readonly IEventBus _eventBus;

    public UpdateProductCommandHandler(
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

    public async Task<Result<ProductDto>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);

        if (product is null)
        {
            return Result.Failure<ProductDto>("Product not found.");
        }

        try
        {
            product.Update(
                request.Name,
                request.Description,
                request.Price,
                request.StockQuantity,
                request.Status);
        }
        catch (ArgumentException exception)
        {
            return Result.Failure<ProductDto>(exception.Message);
        }

        _productRepository.Update(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _productCacheService.RemoveProductsAsync(cancellationToken);

        await _eventBus.PublishAsync(
            new ProductUpdatedIntegrationEvent(
                product.Id,
                product.Name,
                product.Price,
                DateTime.UtcNow),
            cancellationToken);

        return Result.Success(product.ToDto());
    }
}