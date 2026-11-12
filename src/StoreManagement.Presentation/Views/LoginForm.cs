using System.Drawing;
using System.Windows.Forms;
using StoreManagement.Presentation.Theme;

namespace StoreManagement.Presentation.Views;

public sealed class LoginForm : Form
{
    public LoginForm()
    {
        Text = "Connexion";
        StartPosition = FormStartPosition.CenterParent;
        Width = 420;
        Height = 320;
        BackColor = StorePalette.Background;
        Font = StorePalette.DefaultFont;

        var panel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(22),
            ColumnCount = 1,
            RowCount = 5,
            BackColor = StorePalette.Background
        };

        panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        var title = new Label
        {
            Text = "Connexion",
            Font = new Font("Segoe UI", 20F, FontStyle.Bold),
            ForeColor = StorePalette.Primary,
            AutoSize = true,
            Dock = DockStyle.Top
        };

        var username = new TextBox
        {
            PlaceholderText = "Nom d’utilisateur",
            Width = 260,
            Anchor = AnchorStyles.Left | AnchorStyles.Right
        };

        var password = new TextBox
        {
            PlaceholderText = "Mot de passe",
            Width = 260,
            Anchor = AnchorStyles.Left | AnchorStyles.Right,
            UseSystemPasswordChar = true
        };

        var loginButton = new Controls.ModernButton("Se connecter", true);
        loginButton.Click += (_, _) =>
        {
            if (string.IsNullOrWhiteSpace(username.Text) || string.IsNullOrWhiteSpace(password.Text))
            {
                MessageBox.Show("Veuillez remplir les champs de connexion.", "Connexion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            MessageBox.Show($"Connexion simulée pour {username.Text}", "Connexion", MessageBoxButtons.OK, MessageBoxIcon.Information);
        };

        panel.Controls.Add(title, 0, 0);
        panel.Controls.Add(username, 0, 1);
        panel.Controls.Add(password, 0, 2);
        panel.Controls.Add(loginButton, 0, 3);

        Controls.Add(panel);
    }
}
