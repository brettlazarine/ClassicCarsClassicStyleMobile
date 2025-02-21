global using CommunityToolkit;
global using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Shiny;

namespace Client;

public static class MauiProgram
{
    // Toolkit namespace -> xmlns:toolkit="http://schemas.microsoft.com/dotnet/2022/maui/toolkit"
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseShiny()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });
        
#if DEBUG
        builder.Logging.AddDebug();
#endif
        
        builder.Services.AddServices();
        builder.Services.AddViews();

        return builder.Build();
    }

    private static void AddServices(this IServiceCollection services)
    {
        // Shiny services
        services.AddBluetoothLE();
        // The below line will need to be completed if/when GPS is implemented
        //builder.Services.AddGps<Client.Delegates.YourGpsDelegate>();
    }

    private static void AddViews(this IServiceCollection services)
    {
        services.AddSingleton<MainPage>();
    }
}