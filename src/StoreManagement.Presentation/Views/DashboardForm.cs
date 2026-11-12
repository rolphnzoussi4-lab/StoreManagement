using System.Drawing;
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

        var header = new Label
        {
            Text = "Tableau de bord",
            Font = new Font("Segoe UI", 20F, FontStyle.Bold),
            ForeColor = StorePalette.Primary,
            AutoSize = true,
            Dock = DockStyle.Top,
            Padding = new Padding(20, 20, 0, 10)
        };

        var cards = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            Padding = new Padding(20),
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
            BackColor = StorePalette.Background
        };

        cards.Controls.Add(CreateCard("Ventes du jour", "12", StorePalette.Primary));
        cards.Controls.Add(CreateCard("Produits", "48", StorePalette.Success));
        cards.Controls.Add(CreateCard("Stock faible", "3", StorePalette.Warning));
        cards.Controls.Add(CreateCard("Clients", "184", StorePalette.Text));

        var note = new Label
        {
            Text = "Application de gestion de magasin - vue d’ensemble",
            Dock = DockStyle.Bottom,
            Font = new Font("Segoe UI", 10F, FontStyle.Regular),
            ForeColor = StorePalette.Text,
            Padding = new Padding(20, 0, 0, 20)
        };

        Controls.Add(note);
        Controls.Add(cards);
        Controls.Add(header);
    }

    private static Panel CreateCard(string title, string value, Color color)
    {
        var card = new Panel
        {
            Width = 180,
            Height = 120,
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            Padding = new Padding(14)
        };

        var valueLabel = new Label
        {
            Text = value,
            Font = new Font("Segoe UI", 22F, FontStyle.Bold),
            ForeColor = color,
            AutoSize = true,
            Dock = DockStyle.Top
        };

        var labelTitle = new Label
        {
            Text = title,
            Font = new Font("Segoe UI", 10F, FontStyle.Regular),
            ForeColor = StorePalette.Text,
            AutoSize = true,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        };

        card.Controls.Add(labelTitle);
        card.Controls.Add(valueLabel);
        return card;
    }
}
