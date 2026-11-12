using Microsoft.Data.Sqlite;
using StoreManagement.Application.Abstractions.Persistence;
using StoreManagement.Application.Features.Products;
using StoreManagement.Domain.Entities;

namespace StoreManagement.Infrastructure.Persistence.Sqlite.Repositories;

public sealed class ProductRepository : IProductRepository
{
    private readonly SQLiteConnectionFactory _connections;

    public ProductRepository(SQLiteConnectionFactory connections) =>
        _connections = connections ?? throw new ArgumentNullException(nameof(connections));

    public Task<Product?> GetByIdAsync(long productId, CancellationToken cancellationToken = default) =>
        GetOneAsync("produit_id = $id", "$id", productId, cancellationToken);

    public Task<Product?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sku);
        return GetOneAsync("sku = $sku COLLATE NOCASE", "$sku", sku.Trim(), cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> SearchAsync(ProductFilter filter, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(filter);
        var conditions = new List<string>();
        await using var connection = await _connections.CreateOpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();

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

        var where = conditions.Count == 0 ? string.Empty : "WHERE " + string.Join(" AND ", conditions);
        command.CommandText = $"""
            SELECT produit_id, categorie_id, sku, nom, description,
                   prix_vente_centimes, cout_achat_centimes, seuil_alerte_stock,
                   est_actif, cree_le_utc, modifie_le_utc
            FROM Produit {where}
            ORDER BY nom COLLATE NOCASE;
            """;

        var products = new List<Product>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken)) products.Add(Map(reader));
        return products;
    }

    public async Task<long> AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(product);
        product.Validate();
        await using var connection = await _connections.CreateOpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO Produit
                (categorie_id, sku, nom, description, prix_vente_centimes,
                 cout_achat_centimes, stock_quantite, seuil_alerte_stock,
                 est_actif, cree_le_utc, modifie_le_utc)
            VALUES ($categoryId, $sku, $name, $description, $price,
                    $cost, 0, $threshold, $active, $created, $modified);
            """;
        AddProductParameters(command, product);
        await command.ExecuteNonQueryAsync(cancellationToken);
        return connection.LastInsertRowId;
    }

    public async Task UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(product);
        product.Validate();
        await using var connection = await _connections.CreateOpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = """
            UPDATE Produit SET categorie_id = $categoryId, sku = $sku, nom = $name,
                description = $description, prix_vente_centimes = $price,
                cout_achat_centimes = $cost, seuil_alerte_stock = $threshold,
                est_actif = $active, modifie_le_utc = $modified
            WHERE produit_id = $id;
            """;
        AddProductParameters(command, product);
        command.Parameters.AddWithValue("$id", product.ProductId);
        if (await command.ExecuteNonQueryAsync(cancellationToken) == 0)
            throw new KeyNotFoundException($"Produit {product.ProductId} introuvable.");
    }

    private async Task<Product?> GetOneAsync(string predicate, string parameter, object value, CancellationToken cancellationToken)
    {
        await using var connection = await _connections.CreateOpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = $"""
            SELECT produit_id, categorie_id, sku, nom, description,
                   prix_vente_centimes, cout_achat_centimes, seuil_alerte_stock,
                   est_actif, cree_le_utc, modifie_le_utc
            FROM Produit WHERE {predicate};
            """;
        command.Parameters.AddWithValue(parameter, value);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        return await reader.ReadAsync(cancellationToken) ? Map(reader) : null;
    }

    private static void AddProductParameters(SqliteCommand command, Product product)
    {
        command.Parameters.AddWithValue("$categoryId", product.CategoryId);
        command.Parameters.AddWithValue("$sku", product.Sku);
        command.Parameters.AddWithValue("$name", product.Name);
        command.Parameters.AddWithValue("$description", (object?)product.Description ?? DBNull.Value);
        command.Parameters.AddWithValue("$price", product.SalePriceCents);
        command.Parameters.AddWithValue("$cost", (object?)product.PurchaseCostCents ?? DBNull.Value);
        command.Parameters.AddWithValue("$threshold", product.StockAlertThreshold);
        command.Parameters.AddWithValue("$active", product.IsActive ? 1 : 0);
        command.Parameters.AddWithValue("$created", ToDatabaseDate(product.CreatedAtUtc));
        command.Parameters.AddWithValue("$modified", ToDatabaseDate(product.ModifiedAtUtc));
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
        StockAlertThreshold = reader.GetInt32(7),
        IsActive = reader.GetInt32(8) == 1,
        CreatedAtUtc = ReadDate(reader.GetString(9)),
        ModifiedAtUtc = ReadDate(reader.GetString(10))
    };

    private static string ToDatabaseDate(DateTimeOffset value) =>
        value.UtcDateTime.ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'", System.Globalization.CultureInfo.InvariantCulture);

    private static DateTimeOffset ReadDate(string value) =>
        DateTimeOffset.Parse(value, System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.AssumeUniversal | System.Globalization.DateTimeStyles.AdjustToUniversal);
}
