using Microsoft.Data.Sqlite;
using StoreManagement.Application.Abstractions.Persistence;
using StoreManagement.Domain.Entities;

namespace StoreManagement.Infrastructure.Persistence.Sqlite.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly SQLiteConnectionFactory _connections;

    public UserRepository(SQLiteConnectionFactory connections)
    {
        _connections = connections ?? throw new ArgumentNullException(nameof(connections));
    }

    public async Task<User?> GetByIdAsync(long userId, CancellationToken cancellationToken = default)
    {
        await using var connection = await _connections.CreateOpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT utilisateur_id, nom_utilisateur, nom_affiche, courriel, hash_mot_de_passe, est_actif, est_admin FROM Utilisateur WHERE utilisateur_id = $id;";
        command.Parameters.AddWithValue("$id", userId);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        return await reader.ReadAsync(cancellationToken) ? Map(reader) : null;
    }

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        await using var connection = await _connections.CreateOpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT utilisateur_id, nom_utilisateur, nom_affiche, courriel, hash_mot_de_passe, est_actif, est_admin FROM Utilisateur WHERE nom_utilisateur = $username COLLATE NOCASE;";
        command.Parameters.AddWithValue("$username", username.Trim());
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        return await reader.ReadAsync(cancellationToken) ? Map(reader) : null;
    }

    public async Task<long> AddAsync(User user, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);
        user.Validate();

        await using var connection = await _connections.CreateOpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO Utilisateur (nom_utilisateur, nom_affiche, courriel, hash_mot_de_passe, est_actif, est_admin) VALUES ($username, $displayName, $email, $passwordHash, $isActive, $isAdmin);";
        command.Parameters.AddWithValue("$username", user.Username);
        command.Parameters.AddWithValue("$displayName", user.DisplayName);
        command.Parameters.AddWithValue("$email", (object?)user.Email ?? DBNull.Value);
        command.Parameters.AddWithValue("$passwordHash", user.PasswordHash);
        command.Parameters.AddWithValue("$isActive", user.IsActive);
        command.Parameters.AddWithValue("$isAdmin", user.IsAdministrator ? 1 : 0);
        await command.ExecuteNonQueryAsync(cancellationToken);
        return connection.LastInsertRowId;
    }

    private static User Map(SqliteDataReader reader) => new()
    {
        UserId = reader.GetInt64(0),
        Username = reader.GetString(1),
        DisplayName = reader.GetString(2),
        Email = reader.IsDBNull(3) ? null : reader.GetString(3),
        PasswordHash = reader.GetString(4),
        IsActive = reader.GetInt32(5),
        IsAdministrator = reader.GetInt32(6) == 1
    };
}
