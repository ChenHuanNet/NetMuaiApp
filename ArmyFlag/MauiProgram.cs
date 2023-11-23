using Microsoft.Extensions.Logging;
using ZXing.Net.Maui;
using Army.Repository.Sqlite;
using Army.Service;
using Snowflake.Core;
using CommunityToolkit.Maui;
using Army.Domain.Mapper;

namespace ArmyFlag;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkitMediaElement()
            .UseBarcodeReader()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif


        builder.Services.AddArmyRepository();
        builder.Services.AddService();
        builder.Services.AddSingleton<Dilidili>();
        builder.Services.AddSingleton<DilidiliDetail>();
        builder.Services.AddSingleton<DilidiliVideo>();
        builder.Services.AddSingleton(x => new IdWorker(1, 1));

        MapsterConfig.Initialization();


        return builder.Build();
    }
}
