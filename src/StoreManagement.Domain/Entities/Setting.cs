using System.ComponentModel.DataAnnotations;
using StoreManagement.Domain.Enums;

namespace StoreManagement.Domain.Entities;

public sealed class Setting
{
    public long SettingId { get; init; }
    public string Key { get; init; } = string.Empty;
    public string Value { get; init; } = string.Empty;
    public SettingValueType ValueType { get; init; } = SettingValueType.Text;
    public string? Description { get; init; }
    public long? ModifiedByUserId { get; init; }
    public DateTimeOffset ModifiedAtUtc { get; init; } = DateTimeOffset.UtcNow;

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Key) || Key.Length > 150) throw new ValidationException("La clé de configuration est invalide.");
        if (!Enum.IsDefined(ValueType)) throw new ValidationException("Type de valeur invalide.");
    }
}
