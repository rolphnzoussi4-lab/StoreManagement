using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using StoreManagement.Application.Abstractions.Persistence;
using StoreManagement.Application.Features.Products;
using StoreManagement.Infrastructure.Persistence.Sqlite;
using StoreManagement.Infrastructure.Persistence.Sqlite.Repositories;
using StoreManagement.Presentation.Theme;

namespace StoreManagement.Presentation.Views;

public sealed class ProductsForm : Form
{
    private readonly SQLiteConnectionFactory _connectionFactory;
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly DataGridView _grid;

    public ProductsForm()
    {
        Text = "Produits";
        StartPosition = FormStartPosition.CenterParent;
        Width = 920;
        Height = 620;
        BackColor = StorePalette.Background;
        Font = StorePalette.DefaultFont;

        _connectionFactory = new SQLiteConnectionFactory(Path.Combine(AppContext.BaseDirectory, "store.db"));
        _categoryRepository = new CategoryRepository(_connectionFactory);
        _productRepository = new ProductRepository(_connectionFactory);

        var toolbar = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 60,
            Padding = new Padding(8),
            BackColor = StorePalette.Background
        };

        var addButton = new Controls.ModernButton("Nouveau produit", true);
        addButton.Click += async (_, _) => await AddDemoProductAsync();

        var refreshButton = new Controls.ModernButton("Actualiser");
        refreshButton.Click += async (_, _) => await LoadProductsAsync();

        toolbar.Controls.Add(addButton);
        toolbar.Controls.Add(refreshButton);

        _grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AutoGenerateColumns = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            BackgroundColor = StorePalette.Surface,
            BorderStyle = BorderStyle.None,
            ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = StorePalette.Primary,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            }
        };

        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "SKU", DataPropertyName = "Sku" });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Nom", DataPropertyName = "Name" });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Prix", DataPropertyName = "SalePriceCents" });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Stock", DataPropertyName = "StockQuantity" });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Catégorie", DataPropertyName = "CategoryId" });

        var container = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 2,
            ColumnCount = 1,
            Padding = new Padding(8)
        };

        container.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        container.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        container.Controls.Add(toolbar, 0, 0);
        container.Controls.Add(_grid, 0, 1);

        Controls.Add(container);

        _ = InitializeDatabaseAsync();
    }

    private async Task InitializeDatabaseAsync()
    {
        try
        {
            var initializer = new DatabaseInitializer(_connectionFactory);
            await initializer.InitializeAsync();
            await LoadProductsAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erreur d’initialisation SQLite : {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async Task LoadProductsAsync()
    {
        try
        {
            var products = await _productRepository.SearchAsync(new ProductFilter(IsActive: true));
            _grid.DataSource = products.Select(p => new
            {
                p.Sku,
                p.Name,
                SalePriceCents = p.SalePriceCents / 100m,
                p.StockQuantity,
                p.CategoryId
            }).ToList();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erreur de chargement : {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async Task AddDemoProductAsync()
    {
        try
        {
            var service = new ProductService(_productRepository, _categoryRepository);
            var draft = new ProductDraft(
                CategoryId: 1,
                Sku: $"SKU-{DateTime.UtcNow:yyyyMMddHHmmss}",
                Name: "Produit de démonstration",
                Description: "Produit ajouté depuis l’interface.",
                SalePriceCents: 2500,
                PurchaseCostCents: 1800,
                StockAlertThreshold: 5);

            await service.AddAsync(draft);
            await LoadProductsAsync();
            MessageBox.Show("Produit ajouté avec succès.", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
