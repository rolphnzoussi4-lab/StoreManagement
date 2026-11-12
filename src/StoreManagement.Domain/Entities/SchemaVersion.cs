using System.ComponentModel.DataAnnotations;

namespace StoreManagement.Domain.Entities;

public sealed class SchemaVersion
{
    public int Version { get; init; }
    public string Description { get; init; } = string.Empty;
    public string? Checksum { get; init; }
    public DateTimeOffset AppliedAtUtc { get; init; } = DateTimeOffset.UtcNow;

    public void Validate()
    {
        if (Version <= 0) throw new ValidationException("La version du schéma doit être positive.");
        if (string.IsNullOrWhiteSpace(Description) || Description.Length > 1000) throw new ValidationException("La description du schéma est obligatoire.");
    }
}
