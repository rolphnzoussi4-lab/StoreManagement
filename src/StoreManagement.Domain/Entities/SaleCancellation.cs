using System.ComponentModel.DataAnnotations;

namespace StoreManagement.Domain.Entities;

public sealed class SaleCancellation
{
    public long SaleCancellationId { get; init; }
    public long SaleId { get; init; }
    public long UserId { get; init; }
    public string Reason { get; init; } = string.Empty;
    public long RefundedAmountCents { get; init; }
    public DateTimeOffset CancelledAtUtc { get; init; } = DateTimeOffset.UtcNow;

    public void Validate()
    {
        if (SaleId <= 0) throw new ValidationException("L’annulation doit référencer une vente valide.");
        if (UserId <= 0) throw new ValidationException("Un administrateur valide est requis.");
        if (string.IsNullOrWhiteSpace(Reason) || Reason.Trim().Length > 1000) throw new ValidationException("Le motif est obligatoire et limité à 1000 caractères.");
        if (RefundedAmountCents < 0) throw new ValidationException("Le montant remboursé ne peut pas être négatif.");
    }
}
