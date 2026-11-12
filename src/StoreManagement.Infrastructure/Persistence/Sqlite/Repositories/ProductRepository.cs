using Microsoft.Data.Sqlite;
using StoreManagement.Application.Abstractions.Persistence;
using StoreManagement.Domain.Entities;

namespace StoreManagement.Infrastructure.Persistence.Sqlite.Repositories;

public sealed class ProductRepository : IProductRepository
{
    private readonly SQLiteConnectionFactory _connections;

    public ProductRepository(SQLiteConnectionFactory connections)
    {
        _connections = connections ?? throw new ArgumentNullException(nameof(connections));
    }

    public async Task<Product?> GetByIdAsync(long productId, CancellationToken cancellationToken = default)
    {
        await using var connection = await _connections.CreateOpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT produit_id, categorie_id, sku, nom, description, prix_vente_centimes, cout_achat_centimes, stock_quantite, seuil_alerte_stock, est_actif, cree_le_utc, modifie_le_utc FROM Produit WHERE produit_id = $id;";
        command.Parameters.AddWithValue("$id", productId);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        return await reader.ReadAsync(cancellationToken) ? Map(reader) : null;
    }

    public async Task<Product?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default)
    {
        await using var connection = await _connections.CreateOpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT produit_id, categorie_id, sku, nom, description, prix_vente_centimes, cout_achat_centimes, stock_quantite, seuil_alerte_stock, est_actif, cree_le_utc, modifie_le_utc FROM Produit WHERE sku = $sku;";
        command.Parameters.AddWithValue("$sku", sku.Trim());
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        return await reader.ReadAsync(cancellationToken) ? Map(reader) : null;
    }

    public async Task<IReadOnlyList<Product>> SearchAsync(ProductFilter filter, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(filter);
        await using var connection = await _connections.CreateOpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();

        var conditions = new List<string>();
        if (!string.IsNullOrWhiteSpace(filter.SearchText))
        {
            conditions.Add("(sku LIKE $search COLLATE NOCASE OR nom LIKE $search COLLATE NOCASE)");
            command.Parameters.AddWithValue("$search", $"%{filter.SearchText.Trim()}%");
        }
        if (filter.CategoryId.HasValue)
        {
            conditions.Add("categorie_id = $categoryId");
            command.Parameters.AddWithValue("$categoryId", filter.CategoryId.Value);
        }
        if (filter.IsActive.HasValue)
        {
            conditions.Add("est_actif = $isActive");
            command.Parameters.AddWithValue("$isActive", filter.IsActive.Value ? 1 : 0);
        }

        var where = conditions.Count > 0 ? "WHERE " + string.Join(" AND ", conditions) : string.Empty;
        command.CommandText = $"SELECT produit_id, categorie_id, sku, nom, description, prix_vente_centimes, cout_achat_centimes, stock_quantite, seuil_alerte_stock, est_actif, cree_le_utc, modifie_le_utc FROM Produit {where} ORDER BY nom COLLATE NOCASE;";

        var products = new List<Product>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
            products.Add(Map(reader));

        return products;
    }

    public async Task<long> AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(product);
        product.Validate();

        await using var connection = await _connections.CreateOpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO Produit (categorie_id, sku, nom, description, prix_vente_centimes, cout_achat_centimes, stock_quantite, seuil_alerte_stock, est_actif, cree_le_utc, modifie_le_utc) VALUES ($categoryId, $sku, $name, $description, $salePrice, $purchaseCost, $stockQuantity, $threshold, $isActive, $createdAt, $modifiedAt);";
        AddParameters(command, product);
        await command.ExecuteNonQueryAsync(cancellationToken);
        return connection.LastInsertRowId;
    }

    public async Task UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(product);
        product.Validate();

        await using var connection = await _connections.CreateOpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = "UPDATE Produit SET categorie_id = $categoryId, sku = $sku, nom = $name, description = $description, prix_vente_centimes = $salePrice, cout_achat_centimes = $purchaseCost, stock_quantite = $stockQuantity, seuil_alerte_stock = $threshold, est_actif = $isActive, modifie_le_utc = $modifiedAt WHERE produit_id = $id;";
        AddParameters(command, product);
        command.Parameters.AddWithValue("$id", product.ProductId);

        if (await command.ExecuteNonQueryAsync(cancellationToken) == 0)
            throw new KeyNotFoundException($"Produit {product.ProductId} introuvable.");
    }

    private static void AddParameters(SqliteCommand command, Product product)
    {
        command.Parameters.AddWithValue("$categoryId", product.CategoryId);
        command.Parameters.AddWithValue("$sku", product.Sku);
        command.Parameters.AddWithValue("$name", product.Name);
        command.Parameters.AddWithValue("$description", (object?)product.Description ?? DBNull.Value);
        command.Parameters.AddWithValue("$salePrice", product.SalePriceCents);
        command.Parameters.AddWithValue("$purchaseCost", (object?)product.PurchaseCostCents ?? DBNull.Value);
        command.Parameters.AddWithValue("$stockQuantity", product.StockQuantity);
        command.Parameters.AddWithValue("$threshold", product.StockAlertThreshold);
        command.Parameters.AddWithValue("$isActive", product.IsActive ? 1 : 0);
        command.Parameters.AddWithValue("$createdAt", ToDatabaseDate(product.CreatedAtUtc));
        command.Parameters.AddWithValue("$modifiedAt", ToDatabaseDate(product.ModifiedAtUtc));
    }

    private static Product Map(SqliteDataReader reader) => new()
    {
        ProductId = reader.GetInt64(0),
        CategoryId = reader.GetInt64(1),
        Sku = reader.GetString(2),
        Name = reader.GetString(3),
        Description = reader.IsDBNull(4) ? null : reader.GetString(4),
        SalePriceCents = reader.GetInt64(5),
        PurchaseCostCents = reader.IsDBNull(6) ? null : reader.GetInt64(6),
        StockQuantity = reader.GetInt32(7),
        StockAlertThreshold = reader.GetInt32(8),
        IsActive = reader.GetInt32(9) == 1,
        CreatedAtUtc = ReadDate(reader.GetString(10)),
        ModifiedAtUtc = ReadDate(reader.GetString(11))
    };

    private static string ToDatabaseDate(DateTimeOffset value) =>
        value.UtcDateTime.ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'", System.Globalization.CultureInfo.InvariantCulture);

    private static DateTimeOffset ReadDate(string value) =>
        DateTimeOffset.Parse(value, System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.AssumeUniversal | System.Globalization.DateTimeStyles.AdjustToUniversal);
}
