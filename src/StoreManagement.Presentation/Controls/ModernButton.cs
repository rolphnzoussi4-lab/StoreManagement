using System.Drawing;
using System.Windows.Forms;
using StoreManagement.Presentation.Theme;

namespace StoreManagement.Presentation.Controls;

public sealed class ModernButton : Button
{
    public ModernButton(string text, bool primary = false)
    {
        Text = text;
        Font = StorePalette.DefaultFont;
        AutoSize = true;
        MinimumSize = new Size(120, 38);
        Padding = new Padding(12, 6, 12, 6);
        FlatStyle = FlatStyle.Flat;
        ForeColor = primary ? Color.White : StorePalette.Text;
        BackColor = primary ? StorePalette.Primary : StorePalette.Surface;
        FlatAppearance.BorderSize = 0;
        FlatAppearance.BorderColor = primary ? StorePalette.Primary : StorePalette.Border;
    }
}
