using System.Drawing;
using System.Windows.Forms;

namespace StoreManagement.Presentation.Controls;

public sealed class ModernButton : Button
{
    public ModernButton(string text, bool primary = false)
    {
        Text = text;
        Font = new Font("Segoe UI", 10F, FontStyle.Regular);
        AutoSize = true;
        MinimumSize = new Size(120, 38);
        Padding = new Padding(12, 6, 12, 6);
        FlatStyle = FlatStyle.Flat;
        ForeColor = primary ? Color.White : Color.FromArgb(31, 41, 55);
        BackColor = primary ? Color.FromArgb(37, 99, 235) : Color.White;
        FlatAppearance.BorderSize = 0;
        FlatAppearance.BorderColor = primary ? Color.FromArgb(37, 99, 235) : Color.FromArgb(229, 231, 235);
    }
}
