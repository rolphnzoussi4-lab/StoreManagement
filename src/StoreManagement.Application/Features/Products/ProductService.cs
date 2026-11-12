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

    public async Task<long> AddAsync(ProductDraft draft, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(draft);
        var existing = await _products.GetBySkuAsync(draft.Sku.Trim(), cancellationToken);
        if (existing is not null)
            throw new ValidationException("SKU déjà utilisé.");

        var category = await _categories.GetByIdAsync(draft.CategoryId, cancellationToken);
        if (category is null || !category.IsActive)
            throw new ValidationException("Catégorie invalide ou inactive.");

        var product = new Product
        {
            CategoryId = draft.CategoryId,
            Sku = draft.Sku.Trim(),
            Name = draft.Name.Trim(),
            Description = string.IsNullOrWhiteSpace(draft.Description) ? null : draft.Description.Trim(),
            SalePriceCents = draft.SalePriceCents,
            PurchaseCostCents = draft.PurchaseCostCents,
            StockQuantity = 0,
            StockAlertThreshold = draft.StockAlertThreshold,
            IsActive = true,
            CreatedAtUtc = DateTimeOffset.UtcNow,
            ModifiedAtUtc = DateTimeOffset.UtcNow
        };

        product.Validate();
        return await _products.AddAsync(product, cancellationToken);
    }
}
