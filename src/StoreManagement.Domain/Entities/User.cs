using System.ComponentModel.DataAnnotations;

namespace StoreManagement.Domain.Entities;

public sealed class User
{
    public long UserId { get; init; }
    public string Username { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string? Email { get; init; }
    public string PasswordHash { get; init; } = string.Empty;
    public int IsActive { get; init; } = 1;
    public bool IsAdministrator { get; init; }

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Username) || Username.Trim().Length > 100)
            throw new ValidationException("Le nom d'utilisateur est obligatoire et limité à 100 caractères.");

        if (string.IsNullOrWhiteSpace(DisplayName) || DisplayName.Trim().Length > 150)
            throw new ValidationException("Le nom affiché est obligatoire et limité à 150 caractères.");

        if (!string.IsNullOrWhiteSpace(Email) && Email.Length > 254)
            throw new ValidationException("L'email est trop long.");

        if (string.IsNullOrWhiteSpace(PasswordHash))
            throw new ValidationException("Le hash du mot de passe est obligatoire.");
    }
}
