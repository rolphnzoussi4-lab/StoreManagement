using System;
using System.Windows.Forms;
using StoreManagement.Presentation.Views;

namespace StoreManagement.Presentation;

public static class Program
{
    [STAThread]
    public static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new MainShell());
    }
}
