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

        var label = new Label
        {
            Text = "Tableau de bord\nStoreManagement",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = StorePalette.TitleFont,
            ForeColor = StorePalette.Primary
        };

        Controls.Add(label);
    }
}
