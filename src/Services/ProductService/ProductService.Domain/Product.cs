using SharedKernel;
using System;

namespace ProductService.Domain;

public sealed class Product : Entity
{
    private Product()
    {
    }

    private Product(
        Guid id,
        string name,
        string description,
        string sku,
        decimal price,
        int stockQuantity,
        ProductStatus status)
        : base(id)
    {
        Name = name;
        Description = description;
        Sku = sku;
        Price = price;
        StockQuantity = stockQuantity;
        Status = status;
    }

    public string Name { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public string Sku { get; private set; } = string.Empty;

    public decimal Price { get; private set; }

    public int StockQuantity { get; private set; }

    public ProductStatus Status { get; private set; }

    public static Product Create(
        string name,
        string description,
        string sku,
        decimal price,
        int stockQuantity)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Product name is required.", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(sku))
        {
            throw new ArgumentException("Product SKU is required.", nameof(sku));
        }

        if (price <= 0)
        {
            throw new ArgumentException("Product price must be greater than zero.", nameof(price));
        }

        if (stockQuantity < 0)
        {
            throw new ArgumentException("Stock quantity cannot be negative.", nameof(stockQuantity));
        }

        var product = new Product(
            Guid.NewGuid(),
            name.Trim(),
            description.Trim(),
            sku.Trim(),
            price,
            stockQuantity,
            ProductStatus.Active);

        product.RaiseDomainEvent(new ProductCreatedDomainEvent(product.Id, product.Name, product.Price));

        return product;
    }

    public void Update(
        string name,
        string description,
        decimal price,
        int stockQuantity,
        ProductStatus status)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Product name is required.", nameof(name));
        }

        if (price <= 0)
        {
            throw new ArgumentException("Product price must be greater than zero.", nameof(price));
        }

        if (stockQuantity < 0)
        {
            throw new ArgumentException("Stock quantity cannot be negative.", nameof(stockQuantity));
        }

        Name = name.Trim();
        Description = description.Trim();
        Price = price;
        StockQuantity = stockQuantity;
        Status = status;

        SetUpdatedAt();
        RaiseDomainEvent(new ProductUpdatedDomainEvent(Id, Name, Price));
    }

    public void Delete()
    {
        Status = ProductStatus.Deleted;
        SetUpdatedAt();
    }
}