using Microsoft.Data.Sqlite;
using StoreManagement.Application.Abstractions.Persistence;
using StoreManagement.Domain.Entities;

namespace StoreManagement.Infrastructure.Persistence.Sqlite.Repositories;

public sealed class CategoryRepository : ICategoryRepository
{
    private readonly SQLiteConnectionFactory _connections;

    public CategoryRepository(SQLiteConnectionFactory connections)
    {
        _connections = connections ?? throw new ArgumentNullException(nameof(connections));
    }

    public async Task<Category?> GetByIdAsync(long categoryId, CancellationToken cancellationToken = default)
    {
        await using var connection = await _connections.CreateOpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT categorie_id, nom, description, est_active FROM Categorie WHERE categorie_id = $id;";
        command.Parameters.AddWithValue("$id", categoryId);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        if (!await reader.ReadAsync(cancellationToken))
            return null;

        return new Category
        {
            CategoryId = reader.GetInt64(0),
            Name = reader.GetString(1),
            Description = reader.IsDBNull(2) ? null : reader.GetString(2),
            IsActive = reader.GetInt32(3) == 1
        };
    }

    public async Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var categories = new List<Category>();
        await using var connection = await _connections.CreateOpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT categorie_id, nom, description, est_active FROM Categorie ORDER BY nom COLLATE NOCASE;";
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            categories.Add(new Category
            {
                CategoryId = reader.GetInt64(0),
                Name = reader.GetString(1),
                Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                IsActive = reader.GetInt32(3) == 1
            });
        }

        return categories;
    }

    public async Task<long> AddAsync(Category category, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(category);
        category.Validate();

        await using var connection = await _connections.CreateOpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO Categorie (nom, description, est_active, cree_le_utc) VALUES ($name, $description, $isActive, $createdAt);";
        command.Parameters.AddWithValue("$name", category.Name);
        command.Parameters.AddWithValue("$description", (object?)category.Description ?? DBNull.Value);
        command.Parameters.AddWithValue("$isActive", category.IsActive ? 1 : 0);
        command.Parameters.AddWithValue("$createdAt", DateTimeOffset.UtcNow.ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'"));

        await command.ExecuteNonQueryAsync(cancellationToken);
        return connection.LastInsertRowId;
    }
}
