using System.ComponentModel.DataAnnotations;

namespace StoreManagement.Domain.Entities;

public sealed class Product
{
    public long ProductId { get; init; }
    public long CategoryId { get; init; }
    public string Sku { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public long SalePriceCents { get; init; }
    public long? PurchaseCostCents { get; init; }
    public int StockQuantity { get; init; }
    public int StockAlertThreshold { get; init; }
    public bool IsActive { get; init; } = true;
    public DateTimeOffset CreatedAtUtc { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset ModifiedAtUtc { get; init; } = DateTimeOffset.UtcNow;

    public void Validate()
    {
        if (CategoryId <= 0)
            throw new ValidationException("Une catégorie valide est obligatoire.");
        if (string.IsNullOrWhiteSpace(Sku) || Sku.Trim().Length > 80)
            throw new ValidationException("Le SKU est obligatoire et limité à 80 caractères.");
        if (string.IsNullOrWhiteSpace(Name) || Name.Trim().Length > 200)
            throw new ValidationException("Le nom est obligatoire et limité à 200 caractères.");
        if (Description?.Length > 2_000)
            throw new ValidationException("La description ne peut dépasser 2 000 caractères.");
        if (SalePriceCents < 0 || PurchaseCostCents < 0)
            throw new ValidationException("Les montants ne peuvent pas être négatifs.");
        if (StockAlertThreshold < 0)
            throw new ValidationException("Le seuil d’alerte ne peut pas être négatif.");
    }
}
