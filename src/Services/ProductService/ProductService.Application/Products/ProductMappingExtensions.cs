using ProductService.Domain;

namespace ProductService.Application.Products;

public static class ProductMappingExtensions
{
    public static ProductDto ToDto(this Product product)
    {
        return new ProductDto(
            product.Id,
            product.Name,
            product.Description,
            product.Sku,
            product.Price,
            product.StockQuantity,
            product.Status,
            product.CreatedAtUtc,
            product.UpdatedAtUtc);
    }
}