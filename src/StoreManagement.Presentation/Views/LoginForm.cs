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
            Padding = new Padding(20),
            ColumnCount = 1,
            RowCount = 5
        };

        panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        var title = new Label
        {
            Text = "Connexion",
            Font = StorePalette.TitleFont,
            ForeColor = StorePalette.Primary,
            AutoSize = true
        };

        var loginInput = new TextBox { PlaceholderText = "Nom d’utilisateur", Width = 260, Anchor = AnchorStyles.Left | AnchorStyles.Right };
        var passwordInput = new TextBox { PlaceholderText = "Mot de passe", Width = 260, Anchor = AnchorStyles.Left | AnchorStyles.Right, UseSystemPasswordChar = true };
        var loginButton = new Controls.ModernButton("Se connecter", true);
        loginButton.Click += (_, _) => MessageBox.Show($"Connexion simulée pour {loginInput.Text}", "Connexion", MessageBoxButtons.OK, MessageBoxIcon.Information);

        panel.Controls.Add(title, 0, 0);
        panel.Controls.Add(loginInput, 0, 1);
        panel.Controls.Add(passwordInput, 0, 2);
        panel.Controls.Add(loginButton, 0, 3);

        Controls.Add(panel);
    }
}
