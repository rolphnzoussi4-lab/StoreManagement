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

        var tablesExist = await TableExistsAsync(connection, "Utilisateur", cancellationToken);
        if (tablesExist)
            return;

        await using var command = connection.CreateCommand();
        command.CommandText = @"
            CREATE TABLE Utilisateur (
                utilisateur_id INTEGER PRIMARY KEY AUTOINCREMENT,
                nom_utilisateur TEXT NOT NULL UNIQUE,
                nom_affiche TEXT NOT NULL,
                courriel TEXT,
                hash_mot_de_passe TEXT NOT NULL,
                est_actif INTEGER NOT NULL DEFAULT 1,
                est_admin INTEGER NOT NULL DEFAULT 0,
                cree_le_utc TEXT NOT NULL DEFAULT (strftime('%Y-%m-%dT%H:%M:%fZ', 'now'))
            );

            CREATE TABLE Categorie (
                categorie_id INTEGER PRIMARY KEY AUTOINCREMENT,
                nom TEXT NOT NULL,
                description TEXT,
                est_active INTEGER NOT NULL DEFAULT 1,
                cree_le_utc TEXT NOT NULL DEFAULT (strftime('%Y-%m-%dT%H:%M:%fZ', 'now'))
            );

            CREATE TABLE Produit (
                produit_id INTEGER PRIMARY KEY AUTOINCREMENT,
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

            CREATE TABLE Vente (
                vente_id INTEGER PRIMARY KEY AUTOINCREMENT,
                utilisateur_id INTEGER NOT NULL,
                numero_recu TEXT NOT NULL UNIQUE,
                statut TEXT NOT NULL DEFAULT 'Draft',
                methode_paiement TEXT NOT NULL DEFAULT 'Cash',
                sous_total_centimes INTEGER NOT NULL DEFAULT 0,
                remise_centimes INTEGER NOT NULL DEFAULT 0,
                taxe_centimes INTEGER NOT NULL DEFAULT 0,
                total_centimes INTEGER NOT NULL DEFAULT 0,
                cree_le_utc TEXT NOT NULL DEFAULT (strftime('%Y-%m-%dT%H:%M:%fZ', 'now')),
                paye_le_utc TEXT,
                modifie_le_utc TEXT NOT NULL DEFAULT (strftime('%Y-%m-%dT%H:%M:%fZ', 'now')),
                FOREIGN KEY (utilisateur_id) REFERENCES Utilisateur(utilisateur_id)
            );

            CREATE TABLE LigneVente (
                ligne_vente_id INTEGER PRIMARY KEY AUTOINCREMENT,
                vente_id INTEGER NOT NULL,
                produit_id INTEGER NOT NULL,
                quantite INTEGER NOT NULL,
                sku_au_moment_vente TEXT NOT NULL,
                nom_produit_au_moment_vente TEXT NOT NULL,
                prix_unitaire_centimes INTEGER NOT NULL,
                remise_centimes INTEGER NOT NULL DEFAULT 0,
                total_centimes INTEGER NOT NULL,
                FOREIGN KEY (vente_id) REFERENCES Vente(vente_id),
                FOREIGN KEY (produit_id) REFERENCES Produit(produit_id)
            );

            CREATE TABLE MouvementStock (
                mouvement_stock_id INTEGER PRIMARY KEY AUTOINCREMENT,
                produit_id INTEGER NOT NULL,
                utilisateur_id INTEGER,
                ligne_vente_id INTEGER,
                annulation_vente_id INTEGER,
                type_mouvement TEXT NOT NULL,
                variation_quantite INTEGER NOT NULL,
                raison TEXT,
                cree_le_utc TEXT NOT NULL DEFAULT (strftime('%Y-%m-%dT%H:%M:%fZ', 'now')),
                FOREIGN KEY (produit_id) REFERENCES Produit(produit_id)
            );

            CREATE TABLE LogAudit (
                log_audit_id INTEGER PRIMARY KEY AUTOINCREMENT,
                utilisateur_id INTEGER,
                type_entite TEXT NOT NULL,
                identifiant_entite TEXT NOT NULL,
                action TEXT NOT NULL,
                details TEXT,
                cree_le_utc TEXT NOT NULL DEFAULT (strftime('%Y-%m-%dT%H:%M:%fZ', 'now'))
            );

            CREATE TABLE ParametreSysteme (
                parametre_id INTEGER PRIMARY KEY AUTOINCREMENT,
                cle TEXT NOT NULL UNIQUE,
                valeur TEXT NOT NULL,
                type_valeur TEXT NOT NULL,
                description TEXT,
                modifie_par_utilisateur_id INTEGER,
                modifie_le_utc TEXT NOT NULL DEFAULT (strftime('%Y-%m-%dT%H:%M:%fZ', 'now'))
            );

            CREATE TABLE SchemaVersion (
                version INTEGER PRIMARY KEY,
                description TEXT NOT NULL,
                checksum TEXT,
                applique_le_utc TEXT NOT NULL DEFAULT (strftime('%Y-%m-%dT%H:%M:%fZ', 'now'))
            );
        ";

        await command.ExecuteNonQueryAsync(cancellationToken);

        await using var seed = connection.CreateCommand();
        seed.CommandText = @"
            INSERT OR IGNORE INTO Utilisateur (utilisateur_id, nom_utilisateur, nom_affiche, hash_mot_de_passe, est_actif, est_admin)
            VALUES (1, 'admin', 'Administrateur', 'admin:demo', 1, 1);

            INSERT OR IGNORE INTO Categorie (categorie_id, nom, description, est_active)
            VALUES (1, 'Produits généraux', 'Catégorie par défaut du système.', 1);
        ";

        await seed.ExecuteNonQueryAsync(cancellationToken);
    }

    private static async Task<bool> TableExistsAsync(SqliteConnection connection, string tableName, CancellationToken cancellationToken)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT 1 FROM sqlite_master WHERE type = 'table' AND name = $name;";
        command.Parameters.AddWithValue("$name", tableName);
        var result = await command.ExecuteScalarAsync(cancellationToken);
        return result is not null;
    }
}
