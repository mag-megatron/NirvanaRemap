using CommunityToolkit.Maui;
using NirvanaMAUI.Services;
using NirvanaMAUI.ViewModels;

namespace NirvanaMAUI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit().ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                }); ;
            builder.Services.AddSingleton<GamepadRemapService>();
            builder.Services.AddTransient<MainPageViewModel>();
            builder.Services.AddSingleton<MainPage>();// <--- adicione isto

            return builder.Build();
        }
    }
}