using System.Drawing;
using System.Windows.Forms;
using StoreManagement.Presentation.Theme;

namespace StoreManagement.Presentation.Views;

public sealed class SalesForm : Form
{
    public SalesForm()
    {
        Text = "Ventes";
        StartPosition = FormStartPosition.CenterParent;
        Width = 900;
        Height = 560;
        BackColor = StorePalette.Background;
        Font = StorePalette.DefaultFont;

        var toolbar = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 60,
            Padding = new Padding(8),
            BackColor = StorePalette.Background
        };

        var newSale = new Controls.ModernButton("Nouvelle vente", true);
        var refresh = new Controls.ModernButton("Actualiser");
        toolbar.Controls.Add(newSale);
        toolbar.Controls.Add(refresh);

        var grid = new DataGridView
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

        grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Référence", DataPropertyName = "Reference" });
        grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Client", DataPropertyName = "Client" });
        grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Montant", DataPropertyName = "Amount" });
        grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Statut", DataPropertyName = "Status" });

        grid.Rows.Add("VTE-0001", "Client A", "25,00 €", "Payée");
        grid.Rows.Add("VTE-0002", "Client B", "18,50 €", "En cours");
        grid.Rows.Add("VTE-0003", "Client C", "51,20 €", "Payée");

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
