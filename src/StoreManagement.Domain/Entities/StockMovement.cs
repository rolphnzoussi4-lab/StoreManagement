using System.ComponentModel.DataAnnotations;
using StoreManagement.Domain.Enums;

namespace StoreManagement.Domain.Entities;

public sealed class StockMovement
{
    public long StockMovementId { get; init; }
    public long ProductId { get; init; }
    public long? UserId { get; init; }
    public long? SaleLineId { get; init; }
    public long? SaleCancellationId { get; init; }
    public StockMovementType Type { get; init; }
    public int QuantityChange { get; init; }
    public string? Reason { get; init; }
    public DateTimeOffset CreatedAtUtc { get; init; } = DateTimeOffset.UtcNow;

    public void Validate()
    {
        if (ProductId <= 0) throw new ValidationException("Un produit valide est requis.");
        if (!Enum.IsDefined(Type)) throw new ValidationException("Type de mouvement invalide.");
        if (QuantityChange == 0) throw new ValidationException("La variation ne peut pas être nulle.");
    }
}
