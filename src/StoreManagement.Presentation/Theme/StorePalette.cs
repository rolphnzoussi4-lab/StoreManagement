using System.Drawing;
using System.Windows.Forms;

namespace StoreManagement.Presentation.Theme;

public static class StorePalette
{
    public static readonly Color Background = Color.FromArgb(245, 247, 250);
    public static readonly Color Surface = Color.White;
    public static readonly Color Text = Color.FromArgb(31, 41, 55);
    public static readonly Color Primary = Color.FromArgb(37, 99, 235);
    public static readonly Color Danger = Color.FromArgb(185, 28, 28);
    public static readonly Color Border = Color.FromArgb(229, 231, 235);
    public static readonly Color Success = Color.FromArgb(21, 128, 61);
    public static readonly Color Warning = Color.FromArgb(194, 65, 12);
    public static readonly Font DefaultFont = new("Segoe UI", 10F, FontStyle.Regular);
    public static readonly Font TitleFont = new("Segoe UI Semibold", 18F, FontStyle.Bold);
}
