using System.Windows.Forms;
using StoreManagement.Presentation.Theme;

namespace StoreManagement.Presentation.Views;

public sealed class ProductsForm : Form
{
    public ProductsForm()
    {
        Text = "Produits";
        StartPosition = FormStartPosition.CenterParent;
        Width = 920;
        Height = 620;
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

        var grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AutoGenerateColumns = true,
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

        grid.Rows.Add("SKU-001", "Produit 1", "2500", "5");
        grid.Rows.Add("SKU-002", "Produit 2", "3750", "8");

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
        container.Controls.Add(grid, 0, 1);

        Controls.Add(container);
    }
}
