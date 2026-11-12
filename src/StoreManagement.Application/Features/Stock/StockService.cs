using System.ComponentModel.DataAnnotations;
using StoreManagement.Application.Abstractions.Persistence;
using StoreManagement.Domain.Entities;
using StoreManagement.Domain.Enums;

namespace StoreManagement.Application.Features.Stock;

public sealed class StockService
{
    private readonly IProductRepository _products;
    private readonly IStockRepository _stock;

    public StockService(IProductRepository products, IStockRepository stock)
    {
        _products = products ?? throw new ArgumentNullException(nameof(products));
        _stock = stock ?? throw new ArgumentNullException(nameof(stock));
    }

    public async Task<long> RecordMovementAsync(long productId, int quantityChange, StockMovementType type, string reason, long? userId = null, CancellationToken cancellationToken = default)
    {
        if (productId <= 0) throw new ArgumentOutOfRangeException(nameof(productId));
        if (quantityChange == 0) throw new ValidationException("La quantité doit être différente de zéro.");
        if (type == StockMovementType.Entry && quantityChange < 0)
            throw new ValidationException("Une entrée de stock doit avoir une quantité positive.");
        if (type is StockMovementType.Exit or StockMovementType.Sale && quantityChange > 0)
            throw new ValidationException("Une sortie de stock doit avoir une quantité négative.");
        if (string.IsNullOrWhiteSpace(reason) || reason.Trim().Length > 500)
            throw new ValidationException("Le motif est obligatoire et limité à 500 caractères.");

        var product = await _products.GetByIdAsync(productId, cancellationToken)
            ?? throw new KeyNotFoundException($"Produit {productId} introuvable.");
        if (!product.IsActive) throw new ValidationException("Impossible de modifier le stock d’un produit archivé.");

        var movement = new StockMovement
        {
            ProductId = productId,
            UserId = userId,
            Type = type,
            QuantityChange = quantityChange,
            Reason = reason.Trim(),
            CreatedAtUtc = DateTimeOffset.UtcNow
        };
        movement.Validate();
        return await _stock.AddMovementAsync(movement, cancellationToken);
    }
}
