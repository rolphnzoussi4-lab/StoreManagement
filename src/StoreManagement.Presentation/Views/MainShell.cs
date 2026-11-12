using System.Windows.Forms;
using StoreManagement.Presentation.Controls;
using StoreManagement.Presentation.Theme;

namespace StoreManagement.Presentation.Views;

public partial class MainShell : Form
{
    private readonly Panel _contentPanel;

    public MainShell()
    {
        Text = "StoreManagement";
        Width = 1200;
        Height = 800;
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = StorePalette.Background;
        Font = StorePalette.DefaultFont;

        var root = new TableLayoutPanel { Dock = DockStyle.Fill, BackColor = StorePalette.Background, ColumnCount = 2, RowCount = 1, Padding = new Padding(10) };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 220F));
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        var sidebar = new Panel { Dock = DockStyle.Fill, BackColor = StorePalette.Surface, Padding = new Padding(12) };
        var title = new Label { Text = "StoreManagement", Font = StorePalette.TitleFont, AutoSize = true, ForeColor = StorePalette.Primary, Dock = DockStyle.Top };
        var nav = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false, AutoSize = false, Padding = new Padding(0, 12, 0, 0) };

        AddNavigation(nav, "Dashboard", () => new DashboardForm());
        AddNavigation(nav, "Produits", () => new ProductsForm());
        AddNavigation(nav, "Catégories", () => new CategoryForm());
        AddNavigation(nav, "Stock", () => new StockForm());
        AddNavigation(nav, "Ventes", () => new SalesForm());
        AddNavigation(nav, "Connexion", () => new LoginForm());

        sidebar.Controls.Add(title);
        sidebar.Controls.Add(nav);
        _contentPanel = new Panel { Dock = DockStyle.Fill, BackColor = StorePalette.Background };
        root.Controls.Add(sidebar, 0, 0);
        root.Controls.Add(_contentPanel, 1, 0);
        Controls.Add(root);
        ShowPage(new DashboardForm());
    }

    private void AddNavigation(FlowLayoutPanel nav, string text, Func<Form> createPage)
    {
        var button = new ModernButton(text, text == "Dashboard");
        button.Width = 190;
        button.Click += (_, _) => ShowPage(createPage());
        nav.Controls.Add(button);
    }

    private void ShowPage(Form form)
    {
        foreach (Control child in _contentPanel.Controls)
        {
            child.Dispose();
        }
        _contentPanel.Controls.Clear();
        form.TopLevel = false;
        form.FormBorderStyle = FormBorderStyle.None;
        form.Dock = DockStyle.Fill;
        _contentPanel.Controls.Add(form);
        form.Show();
    }
}
