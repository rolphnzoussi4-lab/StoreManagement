using System.ComponentModel.DataAnnotations;
using StoreManagement.Domain.Enums;

namespace StoreManagement.Domain.Entities;

public sealed class AuditLog
{
    public long AuditLogId { get; init; }
    public long? UserId { get; init; }
    public string EntityType { get; init; } = string.Empty;
    public string EntityIdentifier { get; init; } = string.Empty;
    public AuditAction Action { get; init; }
    public string? Details { get; init; }
    public DateTimeOffset CreatedAtUtc { get; init; } = DateTimeOffset.UtcNow;

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(EntityType) || EntityType.Length > 100) throw new ValidationException("Type d'entité invalide.");
        if (string.IsNullOrWhiteSpace(EntityIdentifier) || EntityIdentifier.Length > 100) throw new ValidationException("Identifiant d'entité invalide.");
        if (!Enum.IsDefined(Action)) throw new ValidationException("Action d’audit invalide.");
    }
}
