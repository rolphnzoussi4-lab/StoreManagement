using System;
using System.Drawing;
using System.Windows.Forms;
using StoreManagement.Domain.Entities;
using StoreManagement.Presentation.Theme;

namespace StoreManagement.Presentation.Views;

public sealed class ProductsForm : Form
{
    private readonly DataGridView _grid;

    public ProductsForm()
    {
        Text = "Produits";
        StartPosition = FormStartPosition.CenterParent;
        Width = 980;
        Height = 640;
        BackColor = StorePalette.Background;
        Font = StorePalette.DefaultFont;

        var toolbar = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 60,
            Padding = new Padding(8),
            BackColor = StorePalette.Background
        };

        var addButton = new Controls.ModernButton("Nouveau produit", true);
        var refreshButton = new Controls.ModernButton("Actualiser");
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

        _grid.Rows.Add("SKU-1001", "Produit 1", 2500, 12, 1);
        _grid.Rows.Add("SKU-1002", "Produit 2", 3800, 7, 1);
    }
}
