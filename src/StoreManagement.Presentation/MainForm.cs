using System.Windows.Forms;

namespace StoreManagement.Presentation;

public partial class MainForm : Form
{
    public MainForm()
    {
        Text = "StoreManagement";
        Size = new Size(1200, 800);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.FromArgb(245, 247, 250);

        var label = new Label
        {
            Text = "StoreManagement",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Segoe UI", 24, FontStyle.Bold),
            ForeColor = Color.FromArgb(31, 41, 55)
        };

        Controls.Add(label);
    }
}
