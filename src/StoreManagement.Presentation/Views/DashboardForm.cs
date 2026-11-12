using System.Windows.Forms;
using StoreManagement.Presentation.Theme;

namespace StoreManagement.Presentation.Views;

public sealed class DashboardForm : Form
{
    public DashboardForm()
    {
        Text = "Dashboard";
        StartPosition = FormStartPosition.CenterParent;
        BackColor = StorePalette.Background;
        Font = StorePalette.DefaultFont;

        var panel = new Panel { Dock = DockStyle.Fill, BackColor = StorePalette.Background, Padding = new Padding(24) };
        var label = new Label
        {
            Text = "Tableau de bord\nStoreManagement",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = StorePalette.TitleFont,
            ForeColor = StorePalette.Primary
        };

        panel.Controls.Add(label);
        Controls.Add(panel);
    }
}
