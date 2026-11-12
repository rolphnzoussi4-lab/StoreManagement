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
        grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Total", DataPropertyName = "Total" });
        grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Statut", DataPropertyName = "Status" });

        grid.Rows.Add("VTE-0001", "Client A", "25,00 €", "Payée");
        grid.Rows.Add("VTE-0002", "Client B", "18,50 €", "En cours");

        Controls.Add(grid);
    }
}
