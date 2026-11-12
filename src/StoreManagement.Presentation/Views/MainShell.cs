using System.Drawing;
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

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            BackColor = StorePalette.Background,
            ColumnCount = 2,
            RowCount = 1,
            Padding = new Padding(10)
        };

        root.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 220F));
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

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
            WrapContents = false,
            AutoSize = false,
            Padding = new Padding(0, 12, 0, 0)
        };

        var dashboardButton = new ModernButton("Dashboard", true);
        dashboardButton.Click += (_, _) => ShowPage(new DashboardForm());

        var productsButton = new ModernButton("Produits");
        productsButton.Click += (_, _) => ShowPage(new ProductsForm());

        nav.Controls.Add(dashboardButton);
        nav.Controls.Add(productsButton);

        sidebar.Controls.Add(title);
        sidebar.Controls.Add(nav);

        _contentPanel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = StorePalette.Background
        };

        root.Controls.Add(sidebar, 0, 0);
        root.Controls.Add(_contentPanel, 1, 0);
        Controls.Add(root);

        ShowPage(new DashboardForm());
    }

    private void ShowPage(Form form)
    {
        _contentPanel.Controls.Clear();
        form.TopLevel = false;
        form.FormBorderStyle = FormBorderStyle.None;
        form.Dock = DockStyle.Fill;
        _contentPanel.Controls.Add(form);
        form.Show();
    }
}
