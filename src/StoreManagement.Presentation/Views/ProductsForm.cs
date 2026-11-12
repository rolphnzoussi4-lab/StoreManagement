using System.Windows.Forms;
using StoreManagement.Application.Abstractions.Persistence;
using StoreManagement.Application.Features.Products;
using StoreManagement.Infrastructure.Persistence.Sqlite;
using StoreManagement.Infrastructure.Persistence.Sqlite.Repositories;

namespace StoreManagement.Presentation.Views;

public sealed class ProductsForm : Form
{
    private readonly ProductService _service;

    public ProductsForm()
    {
        Text = "Produits";
        StartPosition = FormStartPosition.CenterParent;
        Width = 900;
        Height = 600;
        BackColor = StoreManagement.Presentation.Theme.StorePalette.Background;

        var db = new SQLiteConnectionFactory(Path.Combine(AppContext.BaseDirectory, "store.db"));
        var repo = new ProductRepository(db);
        var categoryRepo = new CategoryRepository(db);
        _service = new ProductService(repo, categoryRepo);

        var grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AutoGenerateColumns = true,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        };

        var addButton = new Controls.ModernButton("Ajouter produit", true);
        addButton.Click += async (_, _) =>
        {
            var draft = new ProductDraft(
                CategoryId: 1,
                Sku: "SKU-001",
                Name: "Produit test",
                Description: "Produit de démonstration",
                SalePriceCents: 2500,
                PurchaseCostCents: 1800,
                StockAlertThreshold: 5);

            try
            {
                var productId = await _service.AddAsync(draft);
                MessageBox.Show($"Produit créé : {productId}", "OK");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erreur");
            }
        };

        var toolbar = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            Padding = new Padding(8)
        };
        toolbar.Controls.Add(addButton);

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 2,
            ColumnCount = 1,
            Padding = new Padding(8)
        };
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        layout.Controls.Add(toolbar, 0, 0);
        layout.Controls.Add(grid, 0, 1);

        Controls.Add(layout);
    }
}
