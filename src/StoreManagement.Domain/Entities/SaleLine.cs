using System.ComponentModel.DataAnnotations;

namespace StoreManagement.Domain.Entities;

public sealed class SaleLine
{
    public long SaleLineId { get; init; }
    public long SaleId { get; init; }
    public long ProductId { get; init; }
    public int Quantity { get; init; }
    public string SkuAtSale { get; init; } = string.Empty;
    public string ProductNameAtSale { get; init; } = string.Empty;
    public long UnitPriceCents { get; init; }
    public long DiscountCents { get; init; }
    public long TotalCents { get; init; }

    public void Validate()
    {
        if (SaleId <= 0) throw new ValidationException("La ligne doit appartenir à une vente.");
        if (ProductId <= 0) throw new ValidationException("Le produit est obligatoire.");
        if (Quantity <= 0) throw new ValidationException("La quantité doit être supérieure à zéro.");
        if (UnitPriceCents < 0 || DiscountCents < 0 || TotalCents < 0) throw new ValidationException("Les montants doivent être positifs.");
        if (TotalCents != Quantity * UnitPriceCents - DiscountCents) throw new ValidationException("Le total de ligne est incohérent.");
    }
}
