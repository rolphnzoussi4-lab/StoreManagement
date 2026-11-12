using System.Windows.Forms;
using StoreManagement.Presentation.Theme;

namespace StoreManagement.Presentation.Views;

public partial class MainShell : Form
{
    public MainShell()
    {
        Text = "StoreManagement";
        Width = 1200;
        Height = 800;
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = StorePalette.Background;
        Font = StorePalette.DefaultFont;

        var panel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            BackColor = StorePalette.Background,
            ColumnCount = 2,
            RowCount = 1,
            Padding = new Padding(10)
        };

        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 220F));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

        var sidebar = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = StorePalette.Surface,
            Padding = new Padding(12)
        };

        var title = new Label
        {
            Text = "StoreManagement",
            Font = StorePalette.TitleFont,
            AutoSize = true,
            ForeColor = StorePalette.Primary,
            Dock = DockStyle.Top
        };

        var nav = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            AutoSize = false,
            WrapContents = false,
            Padding = new Padding(0, 12, 0, 0)
        };

        var products = new ModernButton("Produits", true);
        products.Click += (_, _) => ShowPage(new ProductsForm());

        nav.Controls.Add(products);
        sidebar.Controls.Add(nav);
        sidebar.Controls.Add(title);

        panel.Controls.Add(sidebar, 0, 0);

        var content = new Panel { Dock = DockStyle.Fill, BackColor = StorePalette.Background };
        panel.Controls.Add(content, 1, 0);

        Controls.Add(panel);
    }

    private void ShowPage(Form form)
    {
        if (Controls.Count > 0) { }
        var current = Controls[0] as TableLayoutPanel;
        if (current is null) return;

        var content = current.GetControlFromPosition(1, 0);
        content.Controls.Clear();
        form.TopLevel = false;
        form.FormBorderStyle = FormBorderStyle.None;
        form.Dock = DockStyle.Fill;
        content.Controls.Add(form);
        form.Show();
    }
}
