namespace StoreManagement.Application.Features.Products;

public sealed record ProductDraft(
    long CategoryId,
    string Sku,
    string Name,
    string? Description,
    long SalePriceCents,
    long? PurchaseCostCents,
    int StockAlertThreshold);
