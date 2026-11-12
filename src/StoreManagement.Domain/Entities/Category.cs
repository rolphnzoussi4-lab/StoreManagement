using System.ComponentModel.DataAnnotations;

namespace StoreManagement.Domain.Entities;

public sealed class Category
{
    public long CategoryId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public bool IsActive { get; init; } = true;
    public DateTimeOffset CreatedAtUtc { get; init; } = DateTimeOffset.UtcNow;

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Name) || Name.Trim().Length > 100)
            throw new ValidationException("Le nom de catégorie est obligatoire et limité à 100 caractères.");
    }
}
