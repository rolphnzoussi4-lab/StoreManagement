using System.ComponentModel.DataAnnotations;
using StoreManagement.Application.Abstractions.Persistence;
using StoreManagement.Domain.Entities;

namespace StoreManagement.Application.Features.Products;

public sealed class ProductService
{
    private readonly IProductRepository _products;
    private readonly ICategoryRepository _categories;

    public ProductService(IProductRepository products, ICategoryRepository categories)
    {
        _products = products ?? throw new ArgumentNullException(nameof(products));
        _categories = categories ?? throw new ArgumentNullException(nameof(categories));
    }

    public Task<IReadOnlyList<Product>> SearchAsync(ProductFilter filter, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(filter);
        return _products.SearchAsync(filter, cancellationToken);
    }

    public async Task<long> AddAsync(ProductDraft draft, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(draft);
        var product = CreateProduct(draft);
        await EnsureActiveCategoryAsync(product.CategoryId, cancellationToken);

        if (await _products.GetBySkuAsync(product.Sku, cancellationToken) is not null)
            throw new ValidationException("Un produit utilise déjà ce SKU.");

        return await _products.AddAsync(product, cancellationToken);
    }

    public async Task UpdateAsync(long productId, ProductDraft draft, CancellationToken cancellationToken = default)
    {
        if (productId <= 0) throw new ArgumentOutOfRangeException(nameof(productId));
        ArgumentNullException.ThrowIfNull(draft);

        var current = await _products.GetByIdAsync(productId, cancellationToken)
            ?? throw new KeyNotFoundException($"Produit {productId} introuvable.");

        var updated = CreateProduct(draft, current);
        await EnsureActiveCategoryAsync(updated.CategoryId, cancellationToken);

        var duplicate = await _products.GetBySkuAsync(updated.Sku, cancellationToken);
        if (duplicate is not null && duplicate.ProductId != productId)
            throw new ValidationException("Un produit utilise déjà ce SKU.");

        await _products.UpdateAsync(updated, cancellationToken);
    }

    public async Task ArchiveAsync(long productId, CancellationToken cancellationToken = default)
    {
        var product = await _products.GetByIdAsync(productId, cancellationToken)
            ?? throw new KeyNotFoundException($"Produit {productId} introuvable.");

        if (!product.IsActive)
            return;

        await _products.UpdateAsync(new Product
        {
            ProductId = product.ProductId,
            CategoryId = product.CategoryId,
            Sku = product.Sku,
            Name = product.Name,
            Description = product.Description,
            SalePriceCents = product.SalePriceCents,
            PurchaseCostCents = product.PurchaseCostCents,
            StockAlertThreshold = product.StockAlertThreshold,
            IsActive = false,
            CreatedAtUtc = product.CreatedAtUtc,
            ModifiedAtUtc = DateTimeOffset.UtcNow
        }, cancellationToken);
    }

    private async Task EnsureActiveCategoryAsync(long categoryId, CancellationToken cancellationToken)
    {
        var category = await _categories.GetByIdAsync(categoryId, cancellationToken);
        if (category is null || !category.IsActive)
            throw new ValidationException("La catégorie est inexistante ou inactive.");
    }

    private static Product CreateProduct(ProductDraft draft, Product? existing = null)
    {
        var product = new Product
        {
            ProductId = existing?.ProductId ?? 0,
            CategoryId = draft.CategoryId,
            Sku = draft.Sku?.Trim() ?? string.Empty,
            Name = draft.Name?.Trim() ?? string.Empty,
            Description = string.IsNullOrWhiteSpace(draft.Description) ? null : draft.Description.Trim(),
            SalePriceCents = draft.SalePriceCents,
            PurchaseCostCents = draft.PurchaseCostCents,
            StockAlertThreshold = draft.StockAlertThreshold,
            IsActive = existing?.IsActive ?? true,
            CreatedAtUtc = existing?.CreatedAtUtc ?? DateTimeOffset.UtcNow,
            ModifiedAtUtc = DateTimeOffset.UtcNow
        };

        product.Validate();
        return product;
    }
}
