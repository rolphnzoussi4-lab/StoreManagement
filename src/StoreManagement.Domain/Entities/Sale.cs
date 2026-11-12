using System.ComponentModel.DataAnnotations;
using StoreManagement.Domain.Enums;

namespace StoreManagement.Domain.Entities;

public sealed class Sale
{
    public long SaleId { get; init; }
    public long UserId { get; init; }
    public string ReceiptNumber { get; init; } = string.Empty;
    public SaleStatus Status { get; init; } = SaleStatus.Draft;
    public SalesPaymentMethod PaymentMethod { get; init; } = SalesPaymentMethod.Cash;
    public long SubtotalCents { get; init; }
    public long DiscountCents { get; init; }
    public long TaxCents { get; init; }
    public long TotalCents { get; init; }
    public DateTimeOffset CreatedAtUtc { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? PaidAtUtc { get; init; }
    public DateTimeOffset ModifiedAtUtc { get; init; } = DateTimeOffset.UtcNow;

    public void Validate()
    {
        if (UserId <= 0) throw new ValidationException("Un utilisateur valide est requis.");
        if (string.IsNullOrWhiteSpace(ReceiptNumber) || ReceiptNumber.Trim().Length > 80) throw new ValidationException("Numéro de reçu invalide.");
        if (!Enum.IsDefined(Status)) throw new ValidationException("Statut invalide.");
        if (SubtotalCents < 0 || DiscountCents < 0 || TaxCents < 0 || TotalCents < 0) throw new ValidationException("Les montants doivent être positifs.");
        if (TotalCents != SubtotalCents - DiscountCents + TaxCents) throw new ValidationException("Le total ne correspond pas au sous-total et aux taxes.");
    }
}
