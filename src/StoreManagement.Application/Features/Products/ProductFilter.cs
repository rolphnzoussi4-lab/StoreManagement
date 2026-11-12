namespace StoreManagement.Application.Features.Products;

public sealed record ProductFilter(
    string? SearchText = null,
    long? CategoryId = null,
    bool? IsActive = true);
