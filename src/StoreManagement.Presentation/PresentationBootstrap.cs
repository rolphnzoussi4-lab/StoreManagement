using System.Windows.Forms;

namespace StoreManagement.Presentation;

public static class PresentationBootstrap
{
    public static void Run()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new StoreManagement.Presentation.Views.MainShell());
    }
}
