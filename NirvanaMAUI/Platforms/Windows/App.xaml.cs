// Platforms/Windows/App.xaml.cs
using Microsoft.Maui;
using Microsoft.Maui.Hosting;

namespace NirvanaMAUI.WinUI;

public partial class App : MauiWinUIApplication
{
    public App() => InitializeComponent();

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
