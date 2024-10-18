using SimpleToolkit.Core;
using SimpleToolkit.SimpleShell;

namespace NavbarAnimation.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("Fredoka-SemiBold.ttf", "BoldFont");
            })
            .UseSimpleToolkit()
            .UseSimpleShell();

#if ANDROID || IOS
        builder.DisplayContentBehindBars();
#endif

#if ANDROID
        builder.SetDefaultStatusBarAppearance(color: Colors.Transparent, lightElements: true);
        builder.SetDefaultNavigationBarAppearance(color: Colors.Transparent);
#endif

        return builder.Build();
    }
}