using System;
using System.Drawing;
using System.IO;
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
        Width = 980;
        Height = 640;
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
        addButton.Click += async (_, _) => await AddProductAsync();

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

        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "SKU", DataPropertyName = nameof(Product.Sku) });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Nom", DataPropertyName = nameof(Product.Name) });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Prix", DataPropertyName = nameof(Product.SalePriceCents) });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Stock", DataPropertyName = nameof(Product.StockQuantity) });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Catégorie", DataPropertyName = nameof(Product.CategoryId) });

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

        Shown += async (_, _) => await InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        try
        {
            await new DatabaseInitializer(_connectionFactory).InitializeAsync();
            await LoadProductsAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erreur d’initialisation : {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            MessageBox.Show($"Erreur de chargement des produits : {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async Task AddProductAsync()
    {
        try
        {
            var categories = await _categoryRepository.GetAllAsync();
            if (categories.Count == 0)
            {
                MessageBox.Show("Aucune catégorie n’est disponible. Créez une catégorie avant d’ajouter un produit.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var dialog = new Form
            {
                Text = "Nouveau produit",
                Width = 460,
                Height = 360,
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            var sku = new TextBox { PlaceholderText = "SKU", Width = 260, Dock = DockStyle.Top, Margin = new Padding(12) };
            var name = new TextBox { PlaceholderText = "Nom du produit", Width = 260, Dock = DockStyle.Top, Margin = new Padding(12) };
            var desc = new TextBox { PlaceholderText = "Description", Width = 260, Dock = DockStyle.Top, Margin = new Padding(12) };
            var price = new NumericUpDown { Width = 200, Minimum = 1, Maximum = 1000000, DecimalPlaces = 2, Dock = DockStyle.Top, Margin = new Padding(12) };
            var stock = new NumericUpDown { Width = 200, Minimum = 0, Maximum = 1000000, Dock = DockStyle.Top, Margin = new Padding(12) };
            var combo = new ComboBox { Width = 200, DropDownStyle = ComboBoxStyle.DropDownList, Dock = DockStyle.Top, Margin = new Padding(12) };
            combo.DataSource = categories;
            combo.DisplayMember = nameof(StoreManagement.Domain.Entities.Category.Name);
            combo.ValueMember = nameof(StoreManagement.Domain.Entities.Category.CategoryId);

            var save = new Controls.ModernButton("Enregistrer", true);
            var cancel = new Controls.ModernButton("Annuler");
            var buttons = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 54, Padding = new Padding(8), FlowDirection = FlowDirection.RightToLeft };
            buttons.Controls.Add(cancel);
            buttons.Controls.Add(save);

            save.Click += async (_, _) =>
            {
                try
                {
                    var service = new ProductService(_productRepository, _categoryRepository);
                    var draft = new ProductDraft(
                        CategoryId: ((StoreManagement.Domain.Entities.Category)combo.SelectedItem).CategoryId,
                        Sku: sku.Text.Trim(),
                        Name: name.Text.Trim(),
                        Description: desc.Text.Trim(),
                        SalePriceCents: (long)(price.Value * 100M),
                        PurchaseCostCents: 0,
                        StockAlertThreshold: 5);

                    await service.AddAsync(draft);
                    await LoadProductsAsync();
                    dialog.DialogResult = DialogResult.OK;
                    dialog.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            cancel.Click += (_, _) => dialog.Close();

            dialog.Controls.Add(buttons);
            dialog.Controls.Add(stock);
            dialog.Controls.Add(price);
            dialog.Controls.Add(desc);
            dialog.Controls.Add(name);
            dialog.Controls.Add(sku);
            dialog.Controls.Add(combo);
            dialog.ShowDialog(this);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
