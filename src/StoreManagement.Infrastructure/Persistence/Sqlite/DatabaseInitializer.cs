using Microsoft.Data.Sqlite;

namespace StoreManagement.Infrastructure.Persistence.Sqlite;

public sealed class DatabaseInitializer
{
    private readonly SQLiteConnectionFactory _connections;

    public DatabaseInitializer(SQLiteConnectionFactory connections)
    {
        _connections = connections ?? throw new ArgumentNullException(nameof(connections));
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await using var connection = await _connections.CreateOpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT name FROM sqlite_master WHERE type = 'table' AND name = 'Produit';";
        var result = await command.ExecuteScalarAsync(cancellationToken);

        if (result is not null)
            return;

        await using var creation = connection.CreateCommand();
        creation.CommandText = @"
            CREATE TABLE Categorie (
                categorie_id INTEGER PRIMARY KEY,
                nom TEXT NOT NULL,
                description TEXT,
                est_active INTEGER NOT NULL DEFAULT 1,
                cree_le_utc TEXT NOT NULL DEFAULT (strftime('%Y-%m-%dT%H:%M:%fZ', 'now'))
            );

            CREATE TABLE Produit (
                produit_id INTEGER PRIMARY KEY,
                categorie_id INTEGER NOT NULL,
                sku TEXT NOT NULL UNIQUE,
                nom TEXT NOT NULL,
                description TEXT,
                prix_vente_centimes INTEGER NOT NULL,
                cout_achat_centimes INTEGER,
                stock_quantite INTEGER NOT NULL DEFAULT 0,
                seuil_alerte_stock INTEGER NOT NULL DEFAULT 0,
                est_actif INTEGER NOT NULL DEFAULT 1,
                cree_le_utc TEXT NOT NULL DEFAULT (strftime('%Y-%m-%dT%H:%M:%fZ', 'now')),
                modifie_le_utc TEXT NOT NULL DEFAULT (strftime('%Y-%m-%dT%H:%M:%fZ', 'now')),
                FOREIGN KEY (categorie_id) REFERENCES Categorie(categorie_id)
            );
        ";
        await creation.ExecuteNonQueryAsync(cancellationToken);
    }
}
