using System.Drawing;
using System.Windows.Forms;
using StoreManagement.Application.Abstractions.Persistence;
using StoreManagement.Application.Features.Products;
using StoreManagement.Application.Features.Stock;
using StoreManagement.Domain.Enums;
using StoreManagement.Infrastructure.Persistence.Sqlite;
using StoreManagement.Infrastructure.Persistence.Sqlite.Repositories;
using StoreManagement.Presentation.Theme;

namespace StoreManagement.Presentation.Views;

public sealed class StockForm : Form
{
    private readonly SQLiteConnectionFactory _connections;
    private readonly IProductRepository _products;
    private readonly StockService _stockService;
    private readonly ComboBox _productBox;
    private readonly NumericUpDown _quantity;
    private readonly TextBox _reason;
    private readonly DataGridView _grid;

    public StockForm()
    {
        Text = "Stock";
        BackColor = StorePalette.Background;
        Font = StorePalette.DefaultFont;
        _connections = new SQLiteConnectionFactory(Path.Combine(AppContext.BaseDirectory, "store.db"));
        _products = new ProductRepository(_connections);
        _stockService = new StockService(_products, new StockRepository(_connections));

        var toolbar = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 112, Padding = new Padding(8), WrapContents = true };
        _productBox = new ComboBox { Width = 240, DropDownStyle = ComboBoxStyle.DropDownList };
        _quantity = new NumericUpDown { Minimum = 1, Maximum = 100000, Width = 110 };
        _reason = new TextBox { Width = 220, PlaceholderText = "Motif" };
        var type = new ComboBox { Width = 130, DropDownStyle = ComboBoxStyle.DropDownList };
        type.Items.Add("Entrée");
        type.Items.Add("Sortie");
        type.SelectedIndex = 0;
        var apply = new Controls.ModernButton("Enregistrer mouvement", true);
        apply.Click += async (_, _) => await ApplyMovementAsync(type.SelectedIndex);
        toolbar.Controls.Add(new Label { Text = "Produit", AutoSize = true, Padding = new Padding(0, 8, 0, 0) });
        toolbar.Controls.Add(_productBox);
        toolbar.Controls.Add(new Label { Text = "Type", AutoSize = true, Padding = new Padding(0, 8, 0, 0) });
        toolbar.Controls.Add(type);
        toolbar.Controls.Add(new Label { Text = "Quantité", AutoSize = true, Padding = new Padding(0, 8, 0, 0) });
        toolbar.Controls.Add(_quantity);
        toolbar.Controls.Add(_reason);
        toolbar.Controls.Add(apply);

        _grid = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoGenerateColumns = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, BackgroundColor = StorePalette.Surface, BorderStyle = BorderStyle.None };
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2, ColumnCount = 1, Padding = new Padding(8) };
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.Controls.Add(toolbar, 0, 0);
        layout.Controls.Add(_grid, 0, 1);
        Controls.Add(layout);
        Shown += async (_, _) => await InitializeAsync();
        _productBox.SelectedIndexChanged += async (_, _) => await LoadMovementsAsync();
    }

    private async Task InitializeAsync()
    {
        try
        {
            await new DatabaseInitializer(_connections).InitializeAsync();
            var products = await _products.SearchAsync(new ProductFilter(IsActive: true));
            _productBox.DataSource = products.ToList();
            _productBox.DisplayMember = nameof(StoreManagement.Domain.Entities.Product.Name);
            _productBox.ValueMember = nameof(StoreManagement.Domain.Entities.Product.ProductId);
            await LoadMovementsAsync();
        }
        catch (Exception ex) { ShowError(ex); }
    }

    private async Task ApplyMovementAsync(int selectedType)
    {
        if (_productBox.SelectedItem is not StoreManagement.Domain.Entities.Product product) return;
        if (string.IsNullOrWhiteSpace(_reason.Text)) { ShowError(new InvalidOperationException("Indique un motif.")); return; }
        try
        {
            var amount = decimal.ToInt32(_quantity.Value);
            var type = selectedType == 0 ? StockMovementType.Entry : StockMovementType.Exit;
            await _stockService.RecordMovementAsync(product.ProductId, type == StockMovementType.Entry ? amount : -amount, type, _reason.Text);
            await LoadMovementsAsync();
            MessageBox.Show("Mouvement enregistré.", "Stock", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex) { ShowError(ex); }
    }

    private async Task LoadMovementsAsync()
    {
        if (_productBox.SelectedItem is not StoreManagement.Domain.Entities.Product product) return;
        try { _grid.DataSource = (await new StockRepository(_connections).GetByProductAsync(product.ProductId)).ToList(); }
        catch (Exception ex) { ShowError(ex); }
    }

    private void ShowError(Exception ex) => MessageBox.Show(ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
}
