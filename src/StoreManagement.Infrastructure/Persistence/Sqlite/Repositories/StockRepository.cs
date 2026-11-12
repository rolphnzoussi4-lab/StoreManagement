using Microsoft.Data.Sqlite;
using StoreManagement.Application.Abstractions.Persistence;
using StoreManagement.Domain.Entities;
using StoreManagement.Domain.Enums;

namespace StoreManagement.Infrastructure.Persistence.Sqlite.Repositories;

public sealed class StockRepository : IStockRepository
{
    private readonly SQLiteConnectionFactory _connections;

    public StockRepository(SQLiteConnectionFactory connections) =>
        _connections = connections ?? throw new ArgumentNullException(nameof(connections));

    public async Task<long> AddMovementAsync(StockMovement movement, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(movement);
        movement.Validate();

        await using var connection = await _connections.CreateOpenConnectionAsync(cancellationToken);
        using var transaction = connection.BeginTransaction();

        await using (var update = connection.CreateCommand())
        {
            update.Transaction = transaction;
            update.CommandText = "UPDATE Produit SET stock_quantite = stock_quantite + $change, modifie_le_utc = $date WHERE produit_id = $id AND stock_quantite + $change >= 0;";
            update.Parameters.AddWithValue("$change", movement.QuantityChange);
            update.Parameters.AddWithValue("$date", ToDatabaseDate(movement.CreatedAtUtc));
            update.Parameters.AddWithValue("$id", movement.ProductId);
            if (await update.ExecuteNonQueryAsync(cancellationToken) == 0)
                throw new InvalidOperationException("Mouvement impossible : produit introuvable ou stock insuffisant.");
        }

        await using (var insert = connection.CreateCommand())
        {
            insert.Transaction = transaction;
            insert.CommandText = "INSERT INTO MouvementStock (produit_id, utilisateur_id, ligne_vente_id, annulation_vente_id, type_mouvement, variation_quantite, raison, cree_le_utc) VALUES ($productId, $userId, $saleLineId, $cancellationId, $type, $change, $reason, $createdAt);";
            insert.Parameters.AddWithValue("$productId", movement.ProductId);
            insert.Parameters.AddWithValue("$userId", (object?)movement.UserId ?? DBNull.Value);
            insert.Parameters.AddWithValue("$saleLineId", (object?)movement.SaleLineId ?? DBNull.Value);
            insert.Parameters.AddWithValue("$cancellationId", (object?)movement.SaleCancellationId ?? DBNull.Value);
            insert.Parameters.AddWithValue("$type", movement.Type.ToString());
            insert.Parameters.AddWithValue("$change", movement.QuantityChange);
            insert.Parameters.AddWithValue("$reason", (object?)movement.Reason ?? DBNull.Value);
            insert.Parameters.AddWithValue("$createdAt", ToDatabaseDate(movement.CreatedAtUtc));
            await insert.ExecuteNonQueryAsync(cancellationToken);
        }

        var id = connection.LastInsertRowId;
        await transaction.CommitAsync(cancellationToken);
        return id;
    }

    public async Task<IReadOnlyList<StockMovement>> GetByProductAsync(long productId, CancellationToken cancellationToken = default)
    {
        var movements = new List<StockMovement>();
        await using var connection = await _connections.CreateOpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT mouvement_stock_id, produit_id, utilisateur_id, ligne_vente_id, annulation_vente_id, type_mouvement, variation_quantite, raison, cree_le_utc FROM MouvementStock WHERE produit_id = $productId ORDER BY mouvement_stock_id DESC;";
        command.Parameters.AddWithValue("$productId", productId);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            if (!Enum.TryParse<StockMovementType>(reader.GetString(5), out var type))
                continue;
            movements.Add(new StockMovement
            {
                StockMovementId = reader.GetInt64(0),
                ProductId = reader.GetInt64(1),
                UserId = reader.IsDBNull(2) ? null : reader.GetInt64(2),
                SaleLineId = reader.IsDBNull(3) ? null : reader.GetInt64(3),
                SaleCancellationId = reader.IsDBNull(4) ? null : reader.GetInt64(4),
                Type = type,
                QuantityChange = reader.GetInt32(6),
                Reason = reader.IsDBNull(7) ? null : reader.GetString(7),
                CreatedAtUtc = DateTimeOffset.Parse(reader.GetString(8), System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal)
            });
        }
        return movements;
    }

    private static string ToDatabaseDate(DateTimeOffset value) =>
        value.UtcDateTime.ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'", System.Globalization.CultureInfo.InvariantCulture);
}
